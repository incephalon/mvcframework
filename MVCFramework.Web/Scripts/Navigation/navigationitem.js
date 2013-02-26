
NavigationItemModel = Backbone.Model.extend({
    defaults: {
        Text: "",
        Url: "",
        Icon: "",
        NavigationID: 0,
        ParentID: null,

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
        /*
                switch (method) {
                    case "create":
                    case 'read':
                    case 'update':
                    case 'delete':
                        options.url += '/' + this.id;
                        break;
                }
                */
        Backbone.sync(method, model, options);
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
        "click #move-up": "moveup",
        "click #move-down": "movedown",
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
        this.parent.trigger("addChild", this.model.get("NavigationID"), this.model.id);
        e.preventDefault();
    },

    moveup: function (e) {
        e.preventDefault();
    },

    movedown: function (e) {
        e.preventDefault();
    },

    edit: function (e) {
        this.model.set({ isEditing: true });
        e.preventDefault();
    },

    cancel: function (e) {
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

        this.model.save();
    },

    delete: function (e) {
        e.preventDefault();
    }

});