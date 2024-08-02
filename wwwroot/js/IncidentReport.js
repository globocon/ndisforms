

$('#btn_submitreport').on('click', function () {

    var form = document.getElementById('ndisir-form');
    var jsformData = new FormData(form);
    //data: jsformData.serialize(),
    //data: $('#frm_add_compliance').serialize(),
    //$('#ndisir-form').serialize()

    var jsdata = {
        'Id': 0,
        'Report_guid': $('#NewIRHeader_Report_guid').val(),
        'Report_number': null,
        'Toi_reportable_incident': $('#NewIRHeader_Toi_reportable_incident_yes').is(':checked'),
        'Toi_ndis': $('#NewIRHeader_Toi_ndis').is(':checked'),
        'Ttoi_other_authority': $('#NewIRHeader_Toi_other_authority').is(':checked'),
        'Toi_other_authority_text': $('#NewIRHeader_Toi_other_authority_text').val(), 
        'Nosmpr_name_of_witness': $('#NewIRHeader_Nosmpr_name_of_witness').val(), 
        'Nosmpr_rpt_rltd_hazard': $('#NewIRHeader_Nosmpr_rpt_rltd_hazard').is(':checked'),
        'Nosmpr_rpt_rltd_nearmiss': $('#NewIRHeader_Nosmpr_rpt_rltd_nearmiss').is(':checked'),
        'Nosmpr_rpt_rltd_incident': $('#NewIRHeader_Nosmpr_rpt_rltd_incident').is(':checked'),
        'Nosmpr_rpt_rltd_concernchange': $('#NewIRHeader_Nosmpr_rpt_rltd_concernchange').is(':checked'),
        'Nosmpr_rpt_dtm': $('#NewIRHeader_Nosmpr_rpt_dtm').val() ?? convertDateFormat($('#NewIRHeader_Nosmpr_rpt_dtm').val(), "yyyy-MM-dd'T'HH:mm:ss"),
        'Nosmpr_rpt_location': $('#NewIRHeader_Nosmpr_rpt_location').val(), 
        'Nosmpr_rpt_rltd_nameofclient': $('#NewIRHeader_Nosmpr_rpt_rltd_nameofclient').val(), 
        'Doibr_text': $('#NewIRHeader_Doibr_text').val(), 
               
    };
    //'RC_CR_DTM': moment().format('YYYY-MM-DDTHH:mm:ss')

    $.ajax({
        url: '/PdfForms/IncidentReportForm?handler=SubmitIncidentReport',
        data: { NewIRHeader: jsdata },
        type: 'POST',
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        beforeSend: function () {
          $('#loader').show();
        }
    }).done(function (result) {
        ShowResult(result);
    }).fail(function (jqXHR, textStatus, errorThrown) {        
        var msg = errorThrown;        
        posbtlf_error_noti(msg);
    }).always(function (jqXHR, textStatus, errorThrown) {
        $('#loader').hide();
    });
});

function ShowResult(result) {
    if (result.status) {        
        var msg = result.message;        
        posbtlf_success_noti(msg);
    }
    else {
        var msg = result.message;       
        posbtlf_error_noti(msg);
    }
}

function convertDateFormat(dte, format) {
    console.log('dte:' + dte);
    if (dte != null) {        
        const { DateTime } = luxon;

        // Convert the selected date
        const selectedDate = DateTime.fromISO(dte);
        const formattedSelectedDate = selectedDate.toFormat(format);
               
        console.log("Formatted Date:", formattedSelectedDate);

        return formattedSelectedDate;
    }
    else {
        return null;
    }
       
}