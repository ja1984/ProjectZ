var issuesModel = function () {
    var priv = {};
    var pub = {};


    priv.Vote = function (data) {
        var inner = {};
        inner.userId = data.UserId;
        return inner;
    };

    priv.Issue = function (data) {
        var inner = {};
        inner.id = data.Id;
        inner.title = data.Title;
        inner.description = data.Description;
        inner.hasVoted = ko.observable(false);
        inner.votes = ko.observableArray([]);

        $.each(data.Votes, function () {
            
            if (!inner.hasVoted() && this.UserId === priv.options.userId)
                inner.hasVoted(true);

            inner.votes.push(new priv.Vote(this));
        });

        inner.allowedToVote = ko.observable(priv.options.userId !== "" && !inner.hasVoted());

        inner.upVote = function () {
            if (!inner.allowedToVote()) return;

            var vote = new priv.Vote();
            inner.votes.push(vote);
            console.log(inner);
            $.ajax({
                url: '/issue/upvote',
                type: 'POST',
                data: { issueId: inner.id },
                dataType: 'json',
                success: function (data) {
                    if (!data.success) {
                        inner.votes.remove(vote);
                    }
                },
                error: function (data) {
                    inner.votes.remove(vote);
                }

            });
        };

        return inner;
    };

    pub.init = function (options) {
        priv.options = options;
        var viewModel = priv.initViewModel();
        return viewModel;
    };

    priv.initViewModel = function () {
        var inner = {};
        inner.issues = ko.observableArray([]);


        $.each(priv.options.data, function () {
            inner.issues.push(new priv.Issue(this));
        });

        console.log(inner);
        return inner;
    };

    return pub;
}();