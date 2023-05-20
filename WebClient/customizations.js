(function(ng) {
    var dependencies = [];

    /*DO NOT MODIFY ABOVE THIS LINE!*/
    /* START DEDAGROUP CUSTOMIZATION */
    dependencies.push('ddg.CopyFunction');
    dependencies.push('ddg.BarcodeScanner');
    dependencies.push('ddg.LocalStorage');
    dependencies.push('ddg.Translate');
    dependencies.push('ddg.PlanetTaxFree');
    dependencies.push('ddg.TransactionsLookup');
	dependencies.push('ddg.GucciCrmIntegration');
	dependencies.push('ddg.GucciVoucherWritePackageNumber');
	
    /* END DEDAGROUP CUSTOMIZATION */

    //Usage example:
    //dependencies.push('dependencyName');

    /*DO NOT MODIFY BELOW THIS LINE!*/
    ng.module('prismApp.customizations', dependencies, null);
})(angular);
