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
        inner.type = data.IssueType;

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

        inner.issueTitle = ko.observable('');
        inner.issueDescription = ko.observable('');
        inner.selectedIssueType = ko.observable(0);
        inner.availableIssueTypes = [{ 'text': 'Bug', 'value': 0 }, { 'text': 'Feature', 'value': 1 }];


        inner.submitIssueButton = ko.computed(function () {
            return inner.issueTitle() === '' || inner.issueDescription() === '';
        });

        inner.resetNewIssue = function() {
            inner.issueTitle('');
            inner.issueDescription('');
            inner.selectedIssueType(0);
            inner.newIssue(false);
        };

        inner.submitIssue = function () {

            $.ajax({
                url: '/Issue/Create',
                method: 'post',
                dataType: 'json',
                data: { 'Title': inner.issueTitle, 'Description': inner.issueDescription, 'IssueType': inner.selectedIssueType().value, 'ProjectId': priv.options.projectId },
                success: function (response) {
                    if (response.success) {
                        var vote = new priv.Vote({ 'UserId': priv.options.userId });
                        var issue = new priv.Issue({ 'Title': inner.issueTitle(), 'Description': inner.issueDescription(), 'IssueType': inner.selectedIssueType().value, 'ProjectId': priv.options.projectId, 'Id': response.message, 'Votes': [vote] });
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