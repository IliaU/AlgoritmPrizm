(function(ng) {
    var dependencies = [];

    /*DO NOT MODIFY ABOVE THIS LINE!*/

    //Usage example:
    //dependencies.push('dependencyName');

  // Add/Remove Comments to Enable/Disable Cayan/Genius EFT Plugin
  dependencies.push('prismPluginsSample.module.cayanRouteModule');
  dependencies.push('prismPluginsSample.service.eftCayanService');
  dependencies.push('prismPluginsSample.controller.cayanDeviceController');
  dependencies.push('prismPluginsSample.controller.cayanGiftCardController');
  dependencies.push('prismPluginsSample.controller.cayanSigCapController');
  dependencies.push('prismPluginsSample.controller.cayanCancelController');


    /*DO NOT MODIFY BELOW THIS LINE!*/
    ng.module('prismApp.customizations', dependencies, null);
})(angular);
