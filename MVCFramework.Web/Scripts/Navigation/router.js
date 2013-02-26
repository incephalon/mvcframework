/* ROUTER */
var NavigationRouter = Backbone.Router.extend({

    initialize: function (options) {
        this.data_loaded = false;
    },

    load_data: function (role) {
        // fetch models and collections
        this.rolesView.collection.fetch();
        
        return true;
    },

    set_urls: function (/*params*/) {
    },

    routes: {
        "": "index",
        "role/:role": "index"
    },
    
    index:function (role) {
        console.log("matched route index:"+role);

        // create views 
        this.rolesView = new RolesView({ el: '#roles-region' });
        this.navigationView = new NavigationView({ el: '#navigation-region' });

        // this.set_urls();
        this.data_loaded = this.data_loaded || this.load_data(role);
        
        if (role) {
            this.navigationView.model.set({ Role: role }, {silent:true});
            this.navigationView.model.fetch();
        }
    }

});

$(function () {
    window.router = new NavigationRouter();
    Backbone.history.start();
});