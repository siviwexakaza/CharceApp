$(function () {

    $("#test").click(function () {
        alert('hello from charce');
    });

    $('.ui.checkbox')
  .checkbox()
    ;

    $("#the_check").on('change', function () {
        if ($(this).parent().closest('div').hasClass('checked')) {
            console.log("On");
        } else {
            console.log("Off");
        }
    });

    $("#btn_add_business").click(function () {
        var BusinessAccountVM = {
            BusinessName: $("#business_name").val(),
            BusinessType: $("#business_type").val(),
            Location: $("#business_location").val(),
            Phone: $("#business_number").val(),
            Email: $("#business_email").val(),
            Website: $("#business_website").val()
        };

        $.post('/profiles/AddProfile', BusinessAccountVM, function (d) {
            console.log(d);
        });
    });
    
    
});



