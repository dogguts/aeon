(function ($) {
    if($.validator && $.validator.unobtrusive){
        var defaultOptions = {
            validClass: 'is-valid',
            errorClass: 'is-invalid',
            highlight: function (element, errorClass, validClass) {
                $(element)
                    .removeClass(validClass)
                    .addClass(errorClass);
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element)
                    .removeClass(errorClass)
                    .addClass(validClass);
            }
        };

        $.validator.setDefaults(defaultOptions);

        $.validator.unobtrusive.options = {
            errorClass: defaultOptions.errorClass,
            validClass: defaultOptions.validClass,
            errorElement: 'div',
            errorPlacement: function (error, element) {
                error.addClass('invalid-feedback');

                if (element.next().is(".input-group-append")) {
                    error.insertAfter(element.next());
                } else {
                    error.insertAfter(element);
                }
            }
        };
    }
    else {
        console.warn('$.validator is not defined. Please load this library **after** loading jquery.validate.js and jquery.validate.unobtrusive.js');
    }
})(jQuery);

/*$("form").validate({
    rules: {
      test: {
        minlength: 3,
        required: true
      }
    },
    showErrors: function(errorMap, errorList) {
      $.each(this.successList, function(index, value) {
        return $(value).popover("hide");
      });
      return $.each(errorList, function(index, value) {
        var _popover;
        console.log(value.message);
        _popover = $(value.element).popover({
          trigger: "manual",
          placement: "top",
          content: value.message,
          template: "<div class=\"popover\"><div class=\"arrow\"></div><div class=\"popover-inner\"><div class=\"popover-content\"><p></p></div></div></div>"
        });
        _popover.data("popover").options.content = value.message;
        return $(value.element).popover("show");
      });
    }
  });
  */


  /*
(function($)
{
	function escapeAttributeValue(value)
	{
		// As mentioned on http://api.jquery.com/category/selectors/
		return value.replace(/([!"#$%&'()*+,./:;<=>?@\[\\\]^`{|}~])/g, "\\$1");
	}

	function addErrorClass(element)
	{
		var group = element.closest('.form-group');
		if (group && group.length > 0)
		{
			group.addClass('has-error').removeClass('has-success');
		}
	}

	function addSuccessClass(element)
	{
		var group = element.closest('.form-group');
		if (group && group.length > 0)
		{
			group.addClass('has-success').removeClass('has-error');
		}
	}

	function onError(formElement, errorPlacementBase, error, inputElement)
	{
		errorPlacementBase(error, inputElement);

		if ($(inputElement).hasClass('input-validation-error'))
		{
			addErrorClass(inputElement)
		}
	}

	function onSuccess(successBase, error)
	{
		var container = error.data("unobtrusiveContainer");

		successBase(error);

		if (container)
		{
			addSuccessClass(container);
		}
	}

	$.fn.validateBootstrap = function(refresh)
	{
		return this.each(function()
		{
			var $this = $(this);
			if (refresh)
			{
				$this.removeData('validator');
				$this.removeData('unobtrusiveValidation');
				$.validator.unobtrusive.parse($this);
			}
			
			var validator = $this.data('validator');

			if (validator)
			{
				validator.settings.errorClass += ' text-danger';
				var errorPlacementBase = validator.settings.errorPlacement;
				var successBase = validator.settings.success;

				validator.settings.errorPlacement = function(error, inputElement)
				{
					onError($this, errorPlacementBase, error, inputElement);
				};

				validator.settings.success = function(error)
				{
					onSuccess(successBase, error);
				}

				$this.find('.input-validation-error').each(function()
				{
					var errorElement = $this.find("[data-valmsg-for='" + escapeAttributeValue($(this)[0].name) + "']");
					var newElement = $(document.createElement(validator.settings.errorElement))
						.addClass('text-danger')
						.attr('for', escapeAttributeValue($(this)[0].name))
						.text(errorElement.text());
					onError($this, errorPlacementBase, newElement, $(this));
				});
			}
			// if validation isn't enabled, but the form has the validation error message element, add error class to container
			else
			{
				$this.find('.input-validation-error').each(function()
				{
					addErrorClass($(this));
				});
			}
		});
	};

	$(function()
	{
		$('form').validateBootstrap();
	});

}(jQuery)); */