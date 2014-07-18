var project = function () {
    var priv = {};
    var pub = {};

    pub.init = function () {
        var viewModel = priv.viewModel();
        return viewModel;
    };

    priv.save = function (data) {

        var postData = {
            project: {
                name: data.name,
                description: data.description
            },
            role: data.role().id
        };

        return $.ajax({
            method: 'post',
            url: '/Project/Create',
            contentType: 'application/json',
            data: ko.toJSON(postData)

        });
    };

    priv.viewModel = function () {
        var inner = {};

        inner.name = ko.observable('');
        inner.description = ko.observable('');
        inner.role = ko.observable('');

        inner.roles = ko.observableArray([
            {
                role: 'Developer',
                id: 0
            },
            {
                role: 'Designer',
                id: 1
            },
            {
                role: 'Administrative',
                id: 2
            },
            {
                role: 'Support',
                id: 3
            },
            {
                role: 'Other',
                id: 4
            }
        
        ]);

        inner.saving = ko.observable(false);

        inner.save = function () {
            inner.saving(true);
            priv.save(inner).done(function (response) {
                if (response.Success) {
                    location.href = response.Url;
                }
            });
        };

        return inner;
    };


    return pub;
}();