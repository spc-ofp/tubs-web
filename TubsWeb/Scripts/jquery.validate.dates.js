/*
 * Validate doesn't reliably work cross-platform, so this
 * is an attempt to force a different rule.
 * Original solution found here:
 * http://christierney.com/2011/06/30/jquery-validate-better-date-method/
 */

jQuery.validator.addMethod(
	"dateSPC",
	function (value, element) {
	    var check = false;
	    // Assumes format of dd-MM-yy HH:mm
	    var re = /^\d{1,2}\-\d{1,2}\-\d{2} \d{2}:\d{2}$/;
	    console.log("validating value: " + value);
	    if (re.test(value)) {
	        var adata = value.split('-');
	        console.log(adata);
	        var dd = parseInt(adata[0], 10);
	        var mm = parseInt(adata[1], 10);
	        var ydata = adata[2].split(' ');
	        var yy = parseInt(ydata[0], 10);
	        var yyyy = 2000 + yy; // This is good for the next 80 years or so
	        var xdata = new Date(yyyy, mm - 1, dd);
	        if ((xdata.getFullYear() == yyyy) && (xdata.getMonth() == mm - 1) && (xdata.getDate() == dd))
	            check = true;
	        else
	            check = false;
	        // TODO: Check the hours
	    } else
	        check = false;
	    return this.optional(element) || check;
	},
	"Please enter a date in the format dd/mm/yy HH:mm"
);
