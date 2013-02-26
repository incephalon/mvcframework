NavigationItemsCollection = Backbone.Collection.extend({
    model: NavigationItemModel
});

NavigationItemsView = Backbone.View.extend({

    initialize: function () {
        this.template = $("#navigation-collection-template").html();
        if (!this.collection) this.collection = new NavigationItemsCollection();
        this.collection.bind("reset", this.render, this);
        this.on("addChild", this.addChild);
        this.on("addSibling", this.addSibling);
        this.on("destroy", this.destroy);
    },

    render: function () {
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

        return this;
    },

    addChild: function (navigationID, parentID) {
        var navChildModel = new NavigationItemModel({ NavigationID: navigationID, ParentID: parentID, isEditing: true });
        var navChildItemView = new NavigationItemView({ model: navChildModel, parent: this });
        this.$el.find('#navigation-item-' + parentID).after(navChildItemView.render().el);

        this.collection.add(navChildModel);
    },

    addSibling: function (navigationID, siblingID) {
        var navSiblingModel = new NavigationItemModel({ NavigationID: navigationID, isEditing: true });
        var navSiblingItemView = new NavigationItemView({ className: "nav-header", model: navSiblingModel, parent: this });
        this.$el.find('#navigation-item-' + siblingID).before(navSiblingItemView.render().el);

        this.collection.add(navSiblingModel);
    },

    // destroys all models with the specified id and its children, also triggers their "destroyed" event
    destroy: function (id) {
        var children = this.collection.where({ ParentID: id });
        var root = this.collection.get(id);
        // if no children, remove the root (header)

        if (children.length == 0) {
            root.destroy({
                success: function (model, response) {
                    model.trigger("destroyed");
                },
                wait: true
            });
        } else //remove all children then the root (header)
            while (children.length > 0) {
                var child = children.pop();
                child.destroy({
                    success: function (model, response) {
                        model.trigger("destroyed");
                        if (children.length == 0)
                            root.destroy({
                                success: function (model, response) {
                                    model.trigger("destroyed");
                                }, wait: true
                            });
                    },
                    wait: true
                });
            }
    }

});