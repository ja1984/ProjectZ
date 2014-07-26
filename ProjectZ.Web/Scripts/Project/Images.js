var imageModel = function() {
    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        var viewModel = new priv.viewModel(data, rootViewModel);

        return viewModel;
    };

    priv.projectImage = function(data) {
        var inner = {};
        inner.logo = ko.observable(data ? data.Logo : '');
        inner.icon = ko.observable(data ? data.Icon : '');
        inner.promo = ko.observable(data ? data.Banner : '');
        return inner;
    };

    priv.image = function(data, viewModel) {
        var inner = {};

        inner.id = ko.observable(data ? data.Id : 0);
        inner.url = ko.observable(data ? data.Url : '');
        inner.thumbnail = ko.observable(data ? data.Thumbnail : '');

        inner.remove = function() {
            
        };

        return inner;
    };
    
    priv.viewModel = function(data, viewModel) {
        var inner = base();
        inner.viewModel = viewModel;
        inner.images = ko.observableArray([]);
        inner.projectId = priv.config.projectId;
        inner.image = ko.observable(new priv.projectImage(data.Image));

        console.log(ko.toJSON(inner.image));

        $.each(data.Images, function() {
            inner.images.push(new priv.image(this, inner));
        });
        
        inner.initFileUpload = function () {
            $('#projectImages').fileupload({
                dropZone: $("#dropzone-project"),
                dataType: 'json',
                done: function (e, data) {
                    if (data.result.success)
                        inner.images.push(new priv.image(data.result.image,inner));
                }
            });
            

            $('#projectPromo').fileupload({
                dropZone: $("#dropzone-promo .dp"),
                dataType: 'json',
                done: function (e, data) {
                    if (data.result.success)
                        inner.image(new priv.projectImage(data.result.image));
                }
            });

        }();

        inner.initMasonry = function() {
            var $container = $('.images-wrapper');
            // initialize
            $container.masonry({
                itemSelector: '.image-item'
            });
        };
        return inner;
    };


    return pub;
};