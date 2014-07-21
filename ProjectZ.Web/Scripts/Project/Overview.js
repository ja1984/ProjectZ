var overviewModel = function (data) {
    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        return new priv.viewModel(data.Description, rootViewModel);
    };

    priv.saveDescription = function (data) {
        
        return $.ajax({
            method: 'post',
            contentType: 'application/json',
            url: '/Project/SaveDescription',
            data: JSON.stringify({ projectId: priv.config.projectId, text: data })
        });
    };

    priv.viewModel = function (data, rootViewModel) {
        var inner = new base();
        inner.description = ko.observable(data);
        inner.viewModel = rootViewModel;
        inner.markdown = ko.observable();
        inner.toggleMarkdown = function () {
            if (!inner.isInMarkdownMode()) {
                inner.previewMarkdown(inner.description()).done(function (response) {
                    inner.markdown(response.Code);
                });
            }
            inner.isInMarkdownMode(!inner.isInMarkdownMode());
        };

        inner.save = function () {
            inner.loading(true);
            priv.saveDescription(inner.description()).done(function (response) {
                inner.loading(false);
                console.log(response);
            });
        };

        return inner;
    };

    return pub;
};