var teamModel = function () {
    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        return new priv.viewModel(data, rootViewModel);
    };

    priv.addMember = function (userId, admin, role) {
        return $.ajax({
            url: '/Project/AddTeamMember',
            method: 'post',
            contentType: 'application/json',
            data: ko.toJSON({ projectId: priv.config.projectId, userId: userId, isAdmin: admin, role: role })
        });
    };

    priv.deleteMember = function (userId) {
        return $.ajax({
            url: '/Project/DeleteTeamMember',
            method: 'post',
            contentType: 'application/json',
            data: ko.toJSON({ projectId: priv.config.projectId, userId: userId })
        });
    };

    priv.teamMember = function (data, viewModel) {
        var inner = {};
        inner.id = ko.observable(data ? data.UserId : '');
        inner.userName = ko.observable(data ? data.UserName : '');
        inner.firstName = ko.observable(data ? data.FirstName: '');
        inner.lastName = ko.observable(data ? data.LastName : '');
        inner.image = ko.observable(data ? data.Image : '');
        inner.isCreator = ko.observable(data ? data.IsCreator : false);
        inner.slug = ko.observable(data ? data.Slug : '');

        inner.remove = function () {
            
            if (!confirm('Are you sure?')) return;

            viewModel.loading(true);
            priv.deleteMember(inner.id()).done(function() {
                viewModel.loading(false);

            });
            viewModel.teamMembers.remove(this);
        };

        return inner;
    };

    priv.viewModel = function (data, viewModel) {
        var inner = base();
        inner.viewModel = viewModel;

        inner.teamMembers = ko.observableArray([]);
        inner.selectedTeamMember = ko.observable();
        inner.role = ko.observable('');
        inner.selectedUser = ko.observable('');
        inner.isPageAdmin = ko.observable(false);
        inner.getOptions = function (searchTerm, callback) {
            $.ajax({
                dataType: "json",
                url: "/User/Search",
                data: {
                    query: searchTerm
                },
            }).done(callback);
        };
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
        inner.addUser = function () {
            if (!inner.selectedUser()) return false;
            inner.errorText('');

            inner.loading(true);
            priv.addMember(inner.selectedUser().UserId, inner.isPageAdmin(), inner.role().id).done(function (response) {
                inner.loading(false);
                if (response.success){
                    inner.teamMembers.push(new priv.teamMember(response.User, inner));
                }
                else {
                    inner.errorText(response.message);
                }
                    
            });
        };
        $.each(data, function () {
            inner.teamMembers.push(new priv.teamMember(this, inner));
        });


        return inner;
    };

    return pub;
};