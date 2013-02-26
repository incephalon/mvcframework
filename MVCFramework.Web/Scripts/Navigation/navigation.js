NavigationModel = Backbone.Model.extend({
    defaults:
    {
        Name: "",
        Role: null
    },

    idAttribute: "ID",

    url: function () {
        var url = "Navigation/GetNavigationForRole";
        if (this.get("Role"))
            url += "?role=" + this.get("Role");
        return url;
    }
});


NavigationView = Backbone.View.extend({

    initialize: function () {
        this.template = $("#navigation-template").html();
        if (!this.model) this.model = new NavigationModel();
        this.model.bind("change", this.render, this);
    },

    render: function () {
        var $content = _.template(this.template, this.model.toJSON());
        this.$el.html($content);

        // render the items collection
        new NavigationItemsView({
            el: "#navigation-container",
            collection: new NavigationItemsCollection(this.model.get("Items"))
        }).render();
    }

});