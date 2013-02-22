RoleModel = Backbone.Model.extend({
    Name: "",
    Title: ""
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

        return this;
    },
    
    navigate: function(e) {
        window.router.navigate("role/" + this.model.get("Title"), true);
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
        
        console.log("rendering roles collection...");
        
        var $container = this.$('#roles-container');
    
        this.collection.each(function (role) {
            var roleView = new RoleView({ model: role, parent: this });
            $container.append(roleView.render().el);
        });
    }

});