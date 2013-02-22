NavigationItemModel = Backbone.Model.extend({
    defaults: {
        Text: "",
        Url: "",
        Icon: "",
        ParentID: null,

        isEditing: false,
    },

    idAttribute: "ID",
    
    isHeader : function() {
        return this.get("ParentID") == null;
    }

});

NavigationItemsCollection = Backbone.Collection.extend({
    role: null,
    model: NavigationItemModel,
    url: function () {
        var url = "Navigation/GetNavigationForRole";
        if (this.role)
            url += "?role=" + this.role;
        return url;
    }

});

NavigationItemView = Backbone.View.extend({
    tagName: "li",

    initialize: function (options) {
        this.template = $('#navigation-item-template').html();
        this.editTemplate = $('#navigation-item-edit-template').html();
        this.parent = options.parent;
        this.model.bind('change', this.render, this);
    },

    events: {
        "click #add-nav-child": "addChild",
        "click #edit": "edit",
        "click #cancel": "cancel",
        "click #save": "save",
        "click #delete": "delete"
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

        return this;
    },

    addChild: function (e) {
        console.log("adding child...");
        this.parent.trigger("addChild", this.model.id);
        e.preventDefault();
    },

    edit: function (e) {
        this.model.set({ isEditing: true });
        e.preventDefault();
    },

    cancel: function (e) {
        console.log(this.model.isNew());
        if (this.model.isNew()) {
            this.model.destroy();
            this.remove();
        }
        else {
            this.model.set({ isEditing: false });
        }
        e.preventDefault();
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
    },
    
    delete: function(e) {
        e.preventDefault();
    }

});

NavigationView = Backbone.View.extend({

    initialize: function () {
        this.template = $("#navigation-template").html();

        if (!this.collection) this.collection = new NavigationItemsCollection();
        this.collection.bind("reset", this.render, this);
        this.on("addChild", this.addChild);
    },

    render: function () {
        var $content = _.template(this.template, {});
        this.$el.html($content);

        console.log("rendering the navigation collection...");

        var $container = this.$('#navigation-container');

        _.each(this.collection.where({ ParentID: null }), function (rootItem) {
            // for each root item.. render it..
            var navRootItemView = new NavigationItemView({ className: "nav-header", model: rootItem, parent: this });
            $container.append(navRootItemView.render().el);
            // ... and render its children
            _.each(this.collection.where({ ParentID: rootItem.id }), function (item) {
                var navChildItemView = new NavigationItemView({ model: item, parent: navRootItemView });
                $container.append(navChildItemView.render().el);
            });
        }, this);
    },

    setRole: function (role) {
        this.collection.role = role;
    },

    addChild: function (parentID) {
        console.log('adding child to ' + parentID);

        var navChildModel = new NavigationItemModel({ ParentID: parentID, isEditing: true });
        var navChildItemView = new NavigationItemView({ model: navChildModel, parent: this });
        this.$el.find('#navigation-item-' + parentID).after(navChildItemView.render().el);
    }
});