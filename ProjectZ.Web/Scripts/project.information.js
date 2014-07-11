var projectModel = function () {
	var priv = {};
	var pub = {};


	pub.Init = function (config) {
		var viewModel = new priv.ViewModel(config);
		viewModel.InitHistory();
		return viewModel;
	};

	priv.ViewModel = function (config) {
		var inner = {};
		inner.followers = ko.observable(config.followers);
		inner.following = ko.observable(config.following);
		inner.selectedPage = ko.observable(config.startPage);

		inner.changePage = function (a, b, c) {
			var newPage = $(b.currentTarget).attr('data-page');
			if (newPage === inner.selectedPage()) return false;
			history.pushState(newPage, 'test', config.url + '/' + newPage);
			inner.selectedPage(newPage);
		};

		inner.toggleFollow = function () {
			if (!inner.following()) {
				inner.follow();
			} else {
				inner.unFollow();
			}
		};

		inner.InitHistory = function () {
			window.addEventListener('popstate', function (event) {
				inner.selectedPage(event.state);
			});
		};

		inner.follow = function () {
			inner.followers(parseInt(inner.followers()) + 1);
			inner.following(true);
			priv.follow(config.projectId)
				.done(function (response) {
					if (!response) {
						inner.followers(parseInt(inner.followers()) - 1);
						inner.following(false);
					}
				})
				.fail(function (errorResponse) {

				});
		};

		inner.unFollow = function () {
			inner.followers(parseInt(inner.followers()) - 1);
			inner.following(false);
			priv.unFollow(config.projectId)
				.done(function (response) {
					if (!response) {
						inner.followers(parseInt(inner.followers()) + 1);
						inner.following(true);
					}
				})
				.fail(function (errorResponse) {

				});
		};

		return inner;
	};

	priv.follow = function (projectId) {
		return $.ajax({
			method: 'post',
			url: '/Project/Follow',
			data: { projectId: projectId }
		});
	};

	priv.unFollow = function (projectId) {
		return $.ajax({
			method: 'post',
			url: '/Project/UnFollow',
			data: { projectId: projectId }
		});
	};

	return pub;
}();