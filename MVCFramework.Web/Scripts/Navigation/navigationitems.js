NavigationItemsCollection = Backbone.Collection.extend({
    model: NavigationItemModel,
    comparator: function (model) {
        return model.get("Order");
    },

    maxIndex: function (parentID) {
        var max = _.max(this.models, function (model) {
            if (model.get("ParentID") == parentID) {
                return 1;
            } else return 0;
        });

        if (max)
            return max.get("Order");
        else return 0;
    }

});

NavigationItemsView = Backbone.View.extend({

    initialize: function (options) {
        this.template = $("#navigation-collection-template").html();

        if (!this.collection) this.collection = new NavigationItemsCollection();
        this.parent = options.parent;

        this.collection.bind("reset", this.render, this);
        this.collection.bind("add", this.itemAdded, this);
        this.collection.bind("remove", this.itemRemoved, this);

        this.on("addSibling", this.addSibling);
        this.on("destroy", this.destroy);
    },

    events: {
        "click #add-first-item": "addFirstItem"
    },

    render: function () {
        var $content = _.template(this.template, {});
        this.$el.html($content);

        var $container = this.$("#navigation-items-container");

        if (this.collection.length == 0) // no navigation items defined
            this.$('#no-items').show();
        else {
            var self = this;
            _.each(this.collection.where({ ParentID: null }), function (rootItem) {
                // for each root item.. render it..
                var navRootItemView = new NavigationItemView({ className: "nav-header", model: rootItem, collection: this.collection, parent: self });
                $container.append(navRootItemView.render().el);
            }, this);
        }
        
        this.$el.sortable({
            items: ".nav-list>li",
            stop: function (evt, ui) {
                console.log($(ui.item).index());
            }
        });

        return this;
    },

    addFirstItem: function (e) {
        e.preventDefault();
        var navRootModel = new NavigationItemModel({ NavigationID: this.parent.model.id, isEditing: true, Order: 1 });
        var navRootView = new NavigationItemView({ className: "nav-header", model: navRootModel, collection: this.collection, parent: this, });
        this.$("#navigation-items-container").append(navRootView.render().el);
        this.collection.add(navRootModel);
    },

    addSibling: function (navigationID, siblingID, order) {
        var navSiblingModel = new NavigationItemModel({ NavigationID: navigationID, isEditing: true, Order: order + 1 });
        var navSiblingItemView = new NavigationItemView({ className: "nav-header", model: navSiblingModel, collection: this.collection, parent: this });
        this.$el.find('#navigation-item-' + siblingID).after(navSiblingItemView.render().el);

        // increase order number for all greater siblings
        _.each(
           _.filter(this.collection.models, function (item) { return item.get("ParentID") == null && item.get("Order") > order; }),
            function (model) {
                model.set({ Order: model.get("Order") + 1 });
            });

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
                    },
                    wait: true
                });
            }
    },

    itemAdded: function (model) {
        this.$('#no-items').hide();
    },

    itemRemoved: function (model) {

        // decrement order for models that have same parent and greater order number
        _.each(
            _.filter(this.collection.models, function (item) { return item.get("ParentID") == model.get("ParentID") && item.get("Order") > model.get("Order"); }, this),
           function (model) { // inner model
               model.set({ Order: model.get("Order") - 1 });
           }, this);

        if (this.collection.length == 0)
            this.$('#no-items').show();
    }

});