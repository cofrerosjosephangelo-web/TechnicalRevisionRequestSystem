function loadRequestDetails ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetRequestDetails",
        type: "GET",
        data: { trrsId: trrsId },

        success: function ( data )
        {
            // =========================
            // HEADER
            // =========================

            $( "#viewControlNo" ).val( data.controlDisplay ?? "" );
            $( "#viewTrrfNo" ).val( data.trrfNo ?? "" );
            $( "#viewDatePrepared" ).val( formatDate( data.datePrepared ) );
            $( "#viewTrrsType" ).val( data.trrsType ?? "" );

            // =========================
            // PRODUCT INFORMATION
            // =========================

            $( "#viewCustomerName" ).val( data.customerName ?? "" );
            $( "#viewProductName" ).val( data.productName ?? "" );
            $( "#viewMoldNo" ).val( data.moldNo ?? "" );
            $( "#viewPartModelNo" ).val( data.partModelNo ?? "" );

            // =========================
            // PROBLEM DETAILS
            // =========================

            $( "#viewEncounteredProblem" ).val( data.encounteredProblem ?? "" );
            $( "#viewMachineName" ).val( data.machineName ?? "" );
            $( "#viewMoldToolLife" ).val( data.moldToolLife ?? "" );

            // =========================
            // ROOT CAUSE
            // =========================

            let rootCauseHtml = "";

            if ( data.rcWearTear )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Wear & Tear</div>`;

            if ( data.rcMachineError )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Machine Error</div>`;

            if ( data.rcDesignError )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Design Error</div>`;

            if ( data.rcFabricationError )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Fabrication Error</div>`;

            if ( data.rcEffectOfPrevImprovement )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Effect of Previous Improvement</div>`;

            if ( data.rcCustomerRequirement )
                rootCauseHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Customer Requirement</div>`;

            if ( rootCauseHtml === "" )
            {
                rootCauseHtml = `<div class="text-slate-400">No Root Cause Selected</div>`;
            }

            $( "#viewRootCauseList" ).html( rootCauseHtml );

            $( "#viewRootCauseDetails" )
                .text( data.rcDetails ?? "" );

            if ( data.rcattachment === true )
            {
                $( "#btnViewRootCauseAttachment" ).removeClass( "hidden" );
            }

            // =========================
            // ACTION PLAN
            // =========================

            let actionPlanHtml = "";

            if ( data.apRepair )
                actionPlanHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Repair</div>`;

            if ( data.apAdjustment )
                actionPlanHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Adjustment</div>`;

            if ( data.apRevision )
                actionPlanHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Revision</div>`;

            if ( data.apReplacement )
                actionPlanHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Replacement</div>`;

            if ( data.apTrialTesting )
                actionPlanHtml += `<div><i class="fa-solid fa-check text-green-600 me-2"></i>Trial / Testing</div>`;

            if ( actionPlanHtml === "" )
            {
                actionPlanHtml = `<div class="text-slate-400">No Action Plan Selected</div>`;
            }

            $( "#viewActionPlanList" ).html( actionPlanHtml );

            $( "#viewActionPlanDetails" )
                .text( data.apDetails ?? "" );

          

            if ( data.apattachment === true )
            {
                $( "#btnViewActionPlanAttachment" ).removeClass( "hidden" );
            }

            // =========================
            // CONTAINMENT
            // =========================

            $( "#viewHasSpare" )
                .text( data.hasSpare ? "Yes" : "No" );

            $( "#viewQuantity" )
                .text( data.quantity ?? "0" );


            if ( data.momattachment === true )
            {
                $( "#btnViewMomAttachment" ).removeClass( "hidden" );
            }

            $( "#viewMomRemarks" )
                .text( data.momremarks ?? "" );




        },

        error: function ( xhr )
        {
            console.error( xhr );

            Swal.fire( {
                icon: "error",
                title: "Unable to Load Request",
                text: "Failed to retrieve TRRF information."
            } );
        }
    } );
}

function formatDate ( dateValue )
{
    if ( !dateValue )
        return "";

    const date = new Date( dateValue );

    const mm = String( date.getMonth() + 1 ).padStart( 2, "0" );
    const dd = String( date.getDate() ).padStart( 2, "0" );
    const yyyy = date.getFullYear();

    return `${ mm }-${ dd }-${ yyyy }`;
}


function loadProductStatus ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetProductStatus",
        type: "GET",
        data: {
            trrsId: trrsId
        },
        success: function ( response )
        {
            if ( !response.success )
            {
                return;
            }

            const data = response.data;

            $( "#viewProductionType" ).text(
                data.is_rush
                    ? "Rush"
                    : data.is_next_production
                        ? "Next Production"
                        : "-"
            );

            $( "#viewStatusAsOf" ).text( formatDate(data.status_as_of) || "-" );
            $( "#viewRequiredDate" ).text( formatDate(data.required_date) || "-" );
            $( "#viewStatusDetails" ).text( data.status_details || "-" );

            $( "#viewProductStatusCard" ).removeClass( "hidden" );
        }
    } );
}


function loadActualInfo ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetActualInfo",
        type: "GET",
        data: {
            trrsId: trrsId
        },
        success: function ( response )
        {
            if ( !response.success )
            {
                return;
            }

            const data = response.data;

            $( "#viewDateReceived" )
                .text( formatDate( data.datereceived ) || "-" );

            $( "#viewDateAccomplishedChange" )
                .text( formatDate( data.dateaccomplishedchange ) || "-" );

            $( "#viewDesignChangeDetails" )
                .text( data.designchangedetails || "-" );

            $( "#viewDateAccomplishedModified" )
                .text( formatDate( data.dateaccomplishedmodified ) || "-" );

            $( "#viewPartModifiedDetails" )
                .text( data.partmodifieddetails || "-" );

            $( "#viewDateAccomplishedResult" )
                .text( formatDate( data.dateaccomplishedresult ) || "-" );

            $( "#viewMoldResult" )
                .text( data.moldresult || "-" );

            $( "#viewActualActivityCard" )
                .removeClass( "hidden" );
        }
    } );
}


function loadProductionSchedule ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetProductionSchedule",
        type: "GET",
        data: {
            trrsId: trrsId
        },
        success: function ( response )
        {
            if ( !response.success )
            {
                $( "#viewProductionScheduleCard" ).addClass( "hidden" );
                return;
            }

            const data = response.data;

            $( "#viewProductionDateReceived" )
                .text( formatDate( data.proddatereceived ) || "-" );

            $( "#viewProductionDateTesting" )
                .text( formatDate( data.prodtesting ) || "-" );

            $( "#viewProductionScheduleCard" )
                .removeClass( "hidden" );
        }
    } );
}

function loadProductResult ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetProductResult",
        type: "GET",
        data: {
            trrsId: trrsId
        },
        success: function ( response )
        {
            if ( !response.success )
            {
                $( "#viewProductResultCard" ).addClass( "hidden" );
                return;
            }

            const data = response.data;

            $( "#viewEvaluationResult" ).text(
                data.isapproved
                    ? "Approved"
                    : "Not Approved"
            );

            $( "#viewProductResult" ).text(
                data.productresult || "-"
            );

            $( "#viewProductResultCard" ).removeClass( "hidden" );
        }
    } );
}


function loadQualityVerification ( trrsId )
{
    $.ajax( {
        url: "/TRRF/GetQualityVerification",
        type: "GET",
        data: {
            trrsId: trrsId
        },
        success: function ( response )
        {
            if ( !response.success )
            {
                $( "#joborderpanel" ).addClass( "hidden" );
                return;
            }

            const data = response.data;

       

            $( "#viewJobOrder" ).text(
                data.job_order || "-"
            );

            $( "#joborderpanel" ).removeClass( "hidden" );
        }
    } );
}