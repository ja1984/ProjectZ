var teamModel = function () {
    var priv = {};
    var pub = {};

    priv.initTypeHead = function () {
        return priv.options.searchBox.typeahead({
            name: 'users',
            valueKey: 'UserName',
            remote: "/user/search?q=%QUERY",
            template: [
                        '<img src="{{Image}}" class="userImage"/>',
                        '<p class="userName">{{UserName}}</p>'
            ].join(''),
            footer: priv.options.footer,
            engine: Hogan
        });
    };

    pub.init = function (options, data) {
        priv.options = options;
        var viewModel = priv.initViewModel(data);
        priv.initTypeHead().on('typeahead:selected', function (obj, datum) {
            viewModel.selectedTeamMember(datum);
        });
        return viewModel;
    };


    priv.TeamMember = function (data) {
        var inner = {};
        console.log(data);
        inner.image = data.Image;
        inner.slug = data.Slug;
        inner.userName = data.UserName;
        inner.role = data.Role;
        inner.firstName = data.FirstName;
        inner.lastName = data.LastName;

        return inner;
    };
    
    priv.addTeamMember = function (teamMember, viewModel) {
        var newTeamMember = priv.TeamMember(teamMember);
        viewModel.teamMembers.push(newTeamMember);
        $.ajax({
            method: 'post',
            url: '/project/addTeamMember?projectID=' + priv.options.projectId,
            data: teamMember,
            dataType: 'json',
            success: function (response) {
                viewModel.selectedTeamMember(null);
                viewModel.addTeamMemberManually(false);
            },
            error: function (response) {
                viewModel.teamMembers.remove(newTeamMember);
            }

        });
    };
    

    priv.initViewModel = function (data) {
        var inner = {};

        inner.teamMembers = ko.observableArray([]);
        inner.selectedTeamMember = ko.observable(null);
        inner.addTeamMemberManually = ko.observable(false);

        inner.userName = ko.observable();

        inner.addTeamMember = function() {
            priv.addTeamMember(inner.selectedTeamMember(),inner);
        };

        inner.setManualAddTeamMember = function() {
            inner.addTeamMemberManually(true);
            priv.options.searchBox.typeahead('setQuery', '');
        };

        $.each(data, function () {
            inner.teamMembers.push(new priv.TeamMember(this));
        });

        return inner;
    };

    return pub;
}();