$(function () {


   

    $("#test").click(function () {
        alert('hello from charce');
    });

    $("html, body").animate({ scrollTop: $("#msgScroll").scrollTop() }, 1000);
    

    $('.ui.checkbox')
  .checkbox()
    ;
    
    $(".the_check").on('change', function () {
        if ($(this).parent().closest('div').hasClass('checked')) {
            
            var id = $(this).attr('name');
            $.post('/Logic/SwitchAccount?BusinessAccID=' + id + '', null, function () {
                window.location.replace("/pages/myaccounts");
            });
            console.log(id);
        } else {
            var id = $(this).attr('name');
            $.post('/Logic/SwitchAccount?BusinessAccID=' + id + '', null, function () {
               window.location.replace("/pages/myaccounts");
            });
            
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
        
        
        $(this).addClass('disabled');
        $(this).addClass('loading');
        
        $.post('/profiles/AddProfile', BusinessAccountVM, function (d) {
            window.location.replace("/pages/myaccounts");
        });
    });

    

    
    
    
});



