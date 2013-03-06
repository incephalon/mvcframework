RoleModel = Backbone.Model.extend({
    defaults: {
        Name: "",
        isActive: false
    }
});

RolesCollection = Backbone.Collection.extend({
    model: RoleModel,
    url: "Roles/GetAllRoles"
});

RoleView = Backbone.View.extend({
    tagName: "li",

    initialize: function () {
        this.template = $('#role-template').html();
        this.model.bind('change', this.render, this);
    },

    events: {
        "click a": "navigate"
    },

    render: function () {
        var $content = _.template(this.template, this.model.toJSON());
        this.$el.html($content);

        if (this.model.get("isActive"))
            this.$el.addClass("active");
        else this.$el.removeClass("active");

        return this;
    },

    navigate: function (e) {
        window.router.navigate("role/" + this.model.get("Name"), true);
        e.preventDefault();
    }

});

RolesView = Backbone.View.extend({

    initialize: function () {

        this.template = $('#roles-template').html();
        if (!this.collection) this.collection = new RolesCollection();
        this.collection.bind("reset", this.render, this);
    },

    render: function () {
        var $content = _.template(this.template, {});
        this.$el.html($content);
        var $container = this.$('#roles-container');

        this.collection.each(function (role) {
            var roleView = new RoleView({ model: role, parent: this });
            $container.append(roleView.render().el);
        });
    },

    setActive: function (role) {
        _.each(this.collection.models, function (model) {
            if (model.get("Name") == role)
                model.set({ isActive: true });
            else
                model.set({ isActive: false });
        });
    }

});