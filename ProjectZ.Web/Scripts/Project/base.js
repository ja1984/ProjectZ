var base = function () {

    var pub = {};
    var priv = {};

    pub.baseModel = function () {
        var inner = {};
        inner.isInEditMode = ko.observable(false);
        inner.isInMarkdownMode = ko.observable(false);

        inner.errorText = ko.observable('');

        inner.hasError = ko.computed(function () {
            return inner.errorText() != '';
        });

        inner.loading = ko.observable(false);

        inner.toggleEditMode = function () {
            inner.isInEditMode(!inner.isInEditMode());
        };

        inner.previewMarkdown = function (data) {
            return $.ajax({
                method: 'post',
                url: '/Home/PreviewMarkdown',
                data: { text: data }
            });
        };

        return inner;
    }();



    return pub.baseModel;
};