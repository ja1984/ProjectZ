ko.bindingHandlers.isotope = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        var $el = $(element);
        var value = ko.utils.unwrapObservable(valueAccessor());

        var $container = $(value.container);

        $container.isotope({
            itemSelector: value.itemSelector
        });

        $container.isotope('appended', $el);

    }

};