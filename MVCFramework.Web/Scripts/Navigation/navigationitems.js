NavigationItemsCollection = Backbone.Collection.extend({
    model: NavigationItemModel
});


NavigationItemsView = Backbone.View.extend({

    initialize: function () {
        this.template = $("#navigation-collection-template").html();
        if (!this.collection) this.collection = new NavigationItemsCollection();
        this.collection.bind("reset", this.render, this);
        this.on("addChild", this.addChild);
    },

    render: function () {
        console.log("rendering the navigation collection...");
        var $content = _.template(this.template, {});
        this.$el.html($content);

        var $container = this.$("#navigation-items-container");

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

  

    addChild: function (navigationID, parentID) {
        console.log('adding child to ' + parentID);

        var navChildModel = new NavigationItemModel({ NavigationID: navigationID, ParentID: parentID, isEditing: true });
        var navChildItemView = new NavigationItemView({ model: navChildModel, parent: this });
        this.$el.find('#navigation-item-' + parentID).after(navChildItemView.render().el);
    }
});