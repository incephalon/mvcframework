
NavigationItemModel = Backbone.Model.extend({
    defaults: {
        Text: "",
        Url: "",
        Icon: "",
        ShowInMenu: false,
        ParentID: null,
        NavigationID: null,
        Order: 0,

        isEditing: false,
    },

    idAttribute: "ID",

    isHeader: function () {
        return this.get("ParentID") == null;
    },

    methodToUrl: {
        "create": "Navigation/CreateNavigationItem",
        "update": "Navigation/UpdateNavigationItem",
        "delete": "Navigation/DeleteNavigationItem",
        "read": "",
    },

    sync: function (method, model, options) {
        options = options || {};
        options.url = model.methodToUrl[method.toLowerCase()];

        switch (method) {
            case 'delete':
                options.url += '/' + this.id;
                break;
        }

        Backbone.sync(method, model, options);
    }

});
var events = {
    "click #add-nav-sibling": "onAddSibling",
    "click #add-nav-child": "onAddChild",
    "click #show-in-menu": "onToggleShowInMenu",
    "click #edit": "onEdit",
    "click #cancel": "onCancel",
    "click #save": "onSave",
    "click #delete": "onDelete",
    "click #preview": "onPreview"
};

NavigationItemView = Backbone.View.extend({
    tagName: "li",

    initialize: function (options) {
        this.template = $('#navigation-item-template').html();
        this.editTemplate = $('#navigation-item-edit-template').html();

        this.parent = options.parent;

        this.model.bind("change", this.render, this);
        this.model.bind("destroyed", this.remove, this);

        // bubble up events
        this.on("destroy", this.destroy, this);
        this.on("sorted", this.sorted, this);
    },
    get events() {
        return events;
    },
    set events(value) {
        events = value;
    },

    render: function () {
        var isEditing = this.model.get("isEditing");
        var $content = _.template(isEditing ? this.editTemplate : this.template, this.model.toJSON());
        this.$el.html($content);
        this.$el.attr("id", "navigation-item-" + this.model.id);

        if (isEditing) {
            // apply form validation
            var $form = this.$el.find('form');
            $form.data("unobtrusiveValidation", null);
            $form.data("validator", null);
            $.validator.unobtrusive.parse($form);
        }

        // render children if any
        var $container = this.$("#navigation-items-container");
        if (this.collection && this.collection.where({ ParentID: this.model.id }).length > 0) {
            var self = this;
            _.each(this.collection.where({ ParentID: this.model.id }), function (item) {
                var navChildItemView = new NavigationItemView({ model: item, parent: self });
                $container.append(navChildItemView.render().el);
            });
        }

        $container.sortable({
            helper: "clone",
            update: _.bind(function (evt, ui) {
                var id = $(ui.item).attr('id').match(/[\d]+$/)[0];
                var position = $(ui.item).index();
                this.parent.trigger("sorted", id, position);
            }, this)
        });

        return this;
    },

    onAddSibling: function (e) {
        e.preventDefault();
        this.parent.trigger("addSibling", this.model.get("NavigationID"), this.model.id, this.model.get("Order"));
    },

    onAddChild: function (e) {
        e.preventDefault();
        var navChildModel = new NavigationItemModel({ NavigationID: this.model.get("NavigationID"), ParentID: this.model.id, Order: 1, isEditing: true });
        var navChildItemView = new NavigationItemView({ model: navChildModel, parent: this });
        var $container = this.$('#navigation-items-container');
        $container.prepend(navChildItemView.render().el);
        $container.sortable("refresh");

        // increase order number for all greater siblings
        _.each(
           _.filter(this.collection.models, function (item) { return item.get("ParentID") == this.model.id; }, this),
            function (model) {
                model.set({ Order: model.get("Order") + 1 });
            }, this);

        this.collection.add(navChildModel);
    },

    onToggleShowInMenu: function (e) {
        e.preventDefault();

        this.model.set({ ShowInMenu: !this.model.get("ShowInMenu") });
        this.model.save();
    },

    onEdit: function (e) {
        this.model.set({ isEditing: true });
        e.preventDefault();
    },

    onCancel: function (e) {
        e.preventDefault();
        if (this.model.isNew()) {
            this.model.destroy();
            this.remove();
        }
        else {
            this.model.set({ isEditing: false });
        }
    },

    onSave: function (e) {
        e.preventDefault();

        var $form = this.$el.find('form');
        if (!$form.valid())
            return;

        this.model.set({
            Icon: this.$('#item-icon').val(),
            Text: this.$('#item-text').val(),
            Url: this.$('#item-url').val(),
            isEditing: false
        });

        this.model.save();
    },

    onDelete: function (e) {
        e.preventDefault();

        if (confirm("Are you sure you want to remove this item and its children?"))
            this.parent.trigger("destroy", this.model.id);

        e.stopPropagation();
    },

    onPreview: function (e) {
        e.preventDefault();
    },

    // bubble up destroy message
    destroy: function (id) {
        this.parent.trigger("destroy", id);
    },

    // bubble up sorted message
    sorted: function (id, position) {
        this.parent.trigger("sorted", id, position);
    }

});