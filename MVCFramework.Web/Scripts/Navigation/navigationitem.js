
NavigationItemModel = Backbone.Model.extend({
    defaults: {
        Text: "",
        Url: "",
        Icon: "",
        ShowInMenu: false,
        ParentID: null,
        NavigationID: null,

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

NavigationItemView = Backbone.View.extend({
    tagName: "li",

    initialize: function (options) {
        this.template = $('#navigation-item-template').html();
        this.editTemplate = $('#navigation-item-edit-template').html();
        this.parent = options.parent;
        this.model.bind("change", this.render, this);
        this.model.bind("destroyed", this.remove, this);
        this.on("destroy", this.destroy, this);
    },

    events: {
        "click #add-nav-sibling": "addSibling",
        "click #add-nav-child": "addChild",
        "click #show-in-menu": "toggleShowInMenu",
        "click #edit": "edit",
        "click #cancel": "cancel",
        "click #save": "save",
        "click #delete": "delete",
        "click #preview": "preview"
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
        } // show tooltips if this is a header
        else if (!this.model.get("ParentID")) {
            this.$('#add-nav-sibling').tooltip();
            this.$('#add-nav-child').tooltip();
            this.$('#show-in-menu').tooltip();
            this.$('#edit').tooltip();
            this.$('#delete').tooltip();
        }

        return this;
    },

    addSibling: function (e) {
        e.preventDefault();
        this.parent.trigger("addSibling", this.model.get("NavigationID"), this.model.id);
    },

    addChild: function (e) {
        e.preventDefault();
        this.parent.trigger("addChild", this.model.get("NavigationID"), this.model.id);
    },

    toggleShowInMenu: function (e) {
        e.preventDefault();
    },

    edit: function (e) {
        this.model.set({ isEditing: true });
        e.preventDefault();
    },

    cancel: function (e) {
        e.preventDefault();
        if (this.model.isNew()) {
            this.model.destroy();
            this.remove();
        }
        else {
            this.model.set({ isEditing: false });
        }
    },

    save: function (e) {
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

    delete: function (e) {
        e.preventDefault();

        if (confirm("Are you sure you want to remove this item and its children?"))
            this.parent.trigger("destroy", this.model.id);
    },
    
    preview: function(e) {
        e.preventDefault();
    },

    // relay a destroy message received from a parent view
    destroy: function (id) {
        this.parent.trigger("destroy", id);
    }

});