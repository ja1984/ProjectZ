var issuesModel = function () {
    var priv = {};
    var pub = {};


    priv.Vote = function (data) {
        var inner = {};
        inner.userId = data.UserId;
        return inner;
    };

    priv.Release = function(data) {
        var inner = {};
        inner.version = data.Version;
        inner.id = data.Id;
        return inner;
    };

    priv.Issue = function (data) {
        var inner = {};
        inner.id = data.Id;
        inner.title = data.Title;
        inner.description = data.Description;
        inner.hasVoted = ko.observable(false);
        inner.comments = data.Comments.length;
        inner.votes = ko.observableArray([]);
        inner.type = data.IssueType;
        inner.typeInText = inner.type == 1 ? "Feature" : "Bug";
        

        inner.issueType = ko.computed(function () {
            return inner.type == 1 ? 'fa-lightbulb-o feature' : 'fa-bug bug';
        });

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
        inner.newIssue = ko.observable(false);

        inner.title = ko.observable('');
        inner.description = ko.observable('');
        inner.selectedIssueType = ko.observable(0);
        inner.type = ko.observable('');
        inner.saving = ko.observable(false);
        inner.types = [{ 'text': 'Bug', 'value': 0 }, { 'text': 'Feature', 'value': 1 }];

        inner.releases = ko.observableArray([]);
        inner.release = ko.observable();


        $.each(priv.options.releases, function() {
            inner.releases.push(new priv.Release(this));
        });


        inner.submitIssueButton = ko.computed(function () {
            return inner.title() === '' || inner.description() === '';
        });

        inner.resetNewIssue = function() {
            inner.title('');
            inner.description('');
            inner.type('Bug');
            inner.newIssue(false);
        };

        inner.save = function () {
            console.log("sabe");
            $.ajax({
                url: '/Issue/Create',
                method: 'post',
                dataType: 'json',
                data: { 'Title': inner.title, 'Description': inner.description, 'IssueType': inner.type(), 'ProjectId': priv.options.projectId },
                success: function (response) {
                    if (response.success) {
                        $('#create-issue').modal('hide');
                        var vote = new priv.Vote({ 'UserId': priv.options.userId });
                        var issue = new priv.Issue({ 'Title': inner.title(), 'Description': inner.description(), 'IssueType': inner.type().value, 'ProjectId': priv.options.projectId, 'Id': response.message, 'Votes': [vote], 'Comments': 0 });
                        inner.issues.push(issue);
                        inner.resetNewIssue();
                    }
                    

                },
                error: function (response) {
                    inner.issues.remove(issue);
                }
            });
        };

        inner.toggleNewIssue = function () {
            inner.newIssue(!inner.newIssue());
        };

        $.each(priv.options.data, function () {
            inner.issues.push(new priv.Issue(this));
        });

        return inner;
    };

    return pub;
}();