var questionModel = function () {
    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        return new priv.viewModel(data.Questions, rootViewModel);
    };

    priv.saveQuestion = function (data) {
        console.log(data(), priv.config.projectId);
        return $.ajax({
            method: 'post',
            url: '/Question/Create',
            contentType: 'application/json',
            data: ko.toJSON({ question: { projectId: priv.config.projectId, title: data().title(), answer: data().answer(), id: data().id() } })
        });
    };

    priv.question = function (data) {
        var inner = {};

        inner.title = ko.observable(data ? data.Title : '');
        inner.answer = ko.observable(data ? data.Answer : '');
        inner.id = ko.observable(data ? data.Id : 0);

        return inner;
    };

    priv.viewModel = function (data, rootViewModel) {
        var inner = new base();
        inner.viewModel = rootViewModel;
        inner.title = ko.observable();
        inner.selectedQuestion = ko.observable();
        inner.answer = ko.observable();
        inner.questions = ko.observableArray([]);

        inner.newQuestion = function () {
            inner.selectedQuestion(new priv.question());
            inner.toggleEditMode();
        };

        inner.editQuestion = function () {
            inner.selectedQuestion(this);
            inner.toggleEditMode();
        };

        inner.save = function () {
            priv.saveQuestion(inner.selectedQuestion).done(function (response) {
                if (response.Success) {
                    if (inner.selectedQuestion().id() == 0)
                        inner.questions.push(new priv.question(response.Question));

                    inner.newQuestion();
                }
            });
        };
        $.each(data, function () {
            inner.questions.push(new priv.question(this));
        });

        return inner;
    };

    return pub;
};