var releaseModel = function (data) {

    var priv = {};
    var pub = {};

    pub.Init = function (data, rootViewModel, config) {
        priv.config = config;
        return new priv.viewModel(data, rootViewModel);
    };

    priv.release = function(data) {
        var inner = {};
        inner.id = ko.observable(data ? data.Id : '');
        inner.version = ko.observable(data ? data.Version : '');
        inner.created = ko.observable(data ? data.Created : '');
        inner.title = ko.observable(data ? data.Title : '');
        inner.description = ko.observable(data ? data.Description : '');
        inner.projectId = ko.observable(data ? data.ProjectId : priv.config.projectId);
        inner.solvedIssues = ko.observableArray([]);
        inner.images = ko.observableArray([]);

        inner.remove = function () {
            console.log(this);
        };

        $.each(data.Issues, function() {
            inner.solvedIssues.push(this.Id);
        });

        return inner;
    };
    
    priv.saveRelease = function (data) {
        return $.ajax({
            method: 'post',
            contentType: 'application/json',
            url: '/Release/Create',
            data: data
        });
    };

    priv.viewModel = function (data, viewModel) {

        var inner = base();
        inner.viewModel = viewModel;
        inner.selectedRelease = ko.observable();
        inner.releases = ko.observableArray([]);
        inner.issues = ko.observableArray([]);

        $.each(data.issues, function () {
            inner.issues.push(this);
        });
        
        $.each(data.releases, function () {
            inner.releases.push(new priv.release(this));
        });

        inner.newRelease = function() {
            inner.selectedRelease(new priv.release({Issues:[], ProjectId: priv.config.projectId}));
            inner.toggleEditMode();
        };

        inner.markdown = ko.observable();
        inner.toggleMarkdown = function () {
            if (!inner.isInMarkdownMode()) {
                inner.previewMarkdown(inner.selectedRelease().description()).done(function (response) {
                    inner.markdown(response.Code);
                });
            }
            inner.isInMarkdownMode(!inner.isInMarkdownMode());
        };

        inner.edit = function () {
            inner.selectedRelease(this);
            console.log(this);
            inner.toggleEditMode();
        };


        inner.save = function () {
            priv.saveRelease(ko.toJSON(inner.selectedRelease())).done(function (response) {
                if (response.Success)
                    if (!inner.selectedRelease().id())
                        inner.releases.push(new priv.release(response.Release));

                inner.toggleEditMode();
            });
        };

        

        inner.initFileUpload = function () {
            priv.config.releaseImages.fileupload({
                dropZone: priv.config.dropzone,
                // Only submit the file if it is jpg file
                add: function (e, data) {
                    var goUpload = true;
                    var uploadFile = data.files[0];
                    // Only allow JPG files
                    if (!(/\.(jpg|png)$/i).test(uploadFile.name)) {
                        alert("Incorrect file format. Please try again.");
                        goUpload = false;
                    }
                    if (goUpload == true) {
                        data.submit();
                    }
                },
                type: 'post',
                // Handles the server side validation on file type
                done: function (e, data) {
                    var response = JSON.parse(data.result);
                    console.log(response);
                    var icon = response.icon;
                    var logo = response.logo;

                    if (response.statusCode == 200) {
                        var logoUrl = '/Uploads/' + priv.config.projectId.replace('/', '') + '/' + logo;
                        var iconUrl = '/Uploads/' + priv.config.projectId.replace('/', '') + '/' + icon;
                        inner.logoUrl(logoUrl);
                        inner.updateLogo(logoUrl, iconUrl);
                    }
                },
                progressall: function (e, data) {
                    // Handle the progress bar
                    var progress = parseInt(data.loaded / data.total * 100, 10);
                    $('#progress .bar').css(
                        'width',
                        progress + '%'
                    );
                }
            });
        }();

        inner.initDropZone = function () {
            $(document).bind('dragover', function (e) {
                var dropZone = priv.config.dropZone,
                    timeout = window.dropZoneTimeout;

                
                if (!timeout) {
                    dropZone.addClass('in');
                } else {
                    clearTimeout(timeout);
                }
                var found = false,
                    node = e.target;
                do {
                    if (node === dropZone[0]) {
                        found = true;
                        break;
                    }
                    node = node.parentNode;
                } while (node != null);
                if (found) {
                    dropZone.addClass('hover');
                } else {
                    dropZone.removeClass('hover');
                }
                window.dropZoneTimeout = setTimeout(function () {
                    window.dropZoneTimeout = null;
                    dropZone.removeClass('in hover');
                }, 100);
            });
        }();

        return inner;
    };

    return pub;
};