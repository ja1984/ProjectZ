var imageModel = function() {
    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        var viewModel = new priv.viewModel(data, rootViewModel);

        return viewModel;
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

    priv.saveImage = function (data) {
        return $.ajax({
            method: 'post',
            url: '/Project/SaveLogo',
            data: { projectId: data.projectId, logo: data.logo, icon: data.icon }
        });
    };

    priv.viewModel = function(data, viewModel) {
        var inner = base();
        inner.viewModel = viewModel;
        inner.images = ko.observableArray([]);
        inner.projectId = priv.config.projectId;


        $.each(data, function() {
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