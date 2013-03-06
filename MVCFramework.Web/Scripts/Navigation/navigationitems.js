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

        this.on("sorted", this.itemSorted);
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
            items: ">ul.nav-list>li",
            helper: "clone",
            update: _.bind(function (evt, ui) {
                var id = $(ui.item).attr('id').match(/[\d]+$/)[0];
                var position = $(ui.item).index();
                this.trigger("sorted", id, position);
            }, this)
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

        var root = this.collection.get(id);

        // remove children
        _.each(this.collection.where({ ParentID: id }), function (child) {
            child.trigger("destroy", child, child.collection);
        });

        // remove item
        root.destroy({
            success: function (model, response) {
                model.trigger("destroyed");
            },
            wait: true
        });
   
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
    },

    itemSorted: function (id, position) {
        var model = this.collection.get(id);
        var order = position + 1; // order starts from 1


        // set order for models in between
        var unordered = _.filter(this.collection.models, function (item) {
            var itemOrder = item.get("Order");
            var hasSameParent = item.get("ParentID") == model.get("ParentID");

            if (order < model.get("Order")) // order decreased
                return hasSameParent && itemOrder >= order && itemOrder < model.get("Order");
            else if (order > model.get("Order")) // order increased
                return hasSameParent && itemOrder > model.get("Order") && itemOrder <= order;
            else return false;
        });

        if (order < model.get("Order")) // order decreased
            _.each(unordered, function (item) {
                item.set({ Order: item.get("Order") + 1 });
            });
        else _.each(unordered, function (item) {
            item.set({ Order: item.get("Order") - 1 });
        });

        model.set({ Order: order });
        model.save();

        // console.log("item sorted: ", id, " ", position);
    }

});