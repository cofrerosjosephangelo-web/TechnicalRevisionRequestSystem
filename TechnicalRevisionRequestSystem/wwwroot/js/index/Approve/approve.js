function markInvalidField ( selector )
{
    const field = $( selector );
    const frame = field.closest( ".flex, .rounded-lg, .rounded-xl" );

    field.focus();

    frame.addClass( "ring-2 ring-red-500 border-red-500" );

    setTimeout( function ()
    {
        frame.removeClass( "ring-2 ring-red-500 border-red-500" );
    }, 2500 );
}

function markInvalidGroup ( selector )
{
    const group = $( selector );

    group.addClass( "ring-2 ring-red-500 rounded-lg" );

    setTimeout( function ()
    {
        group.removeClass( "ring-2 ring-red-500 rounded-lg" );
    }, 2500 );
}



$( document ).on( "change", "#isRush", function ()
{
    if ( $( this ).is( ":checked" ) )
    {
        $( "#isNextProduction" ).prop( "checked", false );
    }
} );

$( document ).on( "change", "#isNextProduction", function ()
{
    if ( $( this ).is( ":checked" ) )
    {
        $( "#isRush" ).prop( "checked", false );
    }
} );

const submitWorkflow = {

    PD: {

        9 ( formData )
        {
            formData.append( "is_rush", $( "#isRush" ).is( ":checked" ) );
            formData.append( "is_next_production", $( "#isNextProduction" ).is( ":checked" ) );
            formData.append( "status_as_of", $( "#statusAsOf" ).val() );
            formData.append( "required_date", $( "#requiredDate" ).val() );
            formData.append( "status_details", $( "#statusDetails" ).val() );
        },

        10 ( formData )
        {
            formData.append( "datereceived", $( "#dateReceived" ).val() );
            formData.append( "dateaccomplishedchange", $( "#dateAccomplishedChange" ).val() );
            formData.append( "designchangedetails", $( "#designChangeDetails" ).val() );
        },

        11 ( formData )
        {
            formData.append( "dateaccomplishedmodified", $( "#dateAccomplishedModified" ).val() );
            formData.append( "partmodifieddetails", $( "#partModifiedDetails" ).val() );
        },

        12 ( formData )
        {
            formData.append( "moldresult", $( "#moldResult" ).val() );
            formData.append( "dateaccomplishedresult", $( "#dateAccomplishedResult" ).val() );
        },

        14 ( formData )
        {
            formData.append( "proddatereceived", $( "#productionDateReceived" ).val() );
            formData.append( "prodtesting", $( "#productionDateTesting" ).val() );
        },

        15 ( formData )
        {
            formData.append( "productresult", $( "#productResult" ).val() );
            formData.append( "isapproved", $( "#isApproved" ).is( ":checked" ) );


            const withDeviation = $( "#withDeviation" ).is( ":checked" );



            formData.append( "deviation_id",
                withDeviation ? deviationno : null );

            formData.append( "deviation_details",
                withDeviation ? $( "#deviationDetails" ).val() : null );

            formData.append( "deviation_date",
                withDeviation ? $( "#deviationDate" ).val() : null );


            if ( withDeviation )
            {

                formData.append( "rcWearTear", $( "#wearTearSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcMachineError", $( "#machineErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcDesignError", $( "#designErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcFabricationError", $( "#fabricationErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcEffectOfPrevImprovement", $( "#prevImpvtSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcCustomerRequirement", $( "#customerReqSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcDetails", $( "#rootCauseDetailsTxtUpdateRev" ).val() );
                formData.append( "apRepair", $( "#repairSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apAdjustment", $( "#adjustmentSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apRevision", $( "#revisionSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apReplacement", $( "#replacementSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apTrialTesting", $( "#trialTestingSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apDetails", $( "#actionPlanDetailsTxtUpdateRev" ).val() );

            }


        },

        16 ( formData )
        {
            formData.append( "job_order", $( "#jobOrdertxt" ).val() );
        }

    },

    MP: {

        8 ( formData )
        {
            formData.append( "is_rush", $( "#isRush" ).is( ":checked" ) );
            formData.append( "is_next_production", $( "#isNextProduction" ).is( ":checked" ) );
            formData.append( "status_as_of", $( "#statusAsOf" ).val() );
            formData.append( "required_date", $( "#requiredDate" ).val() );
            formData.append( "status_details", $( "#statusDetails" ).val() );
        },

        9 ( formData )
        {
            formData.append( "datereceived", $( "#dateReceived" ).val() );
            formData.append( "dateaccomplishedchange", $( "#dateAccomplishedChange" ).val() );
            formData.append( "designchangedetails", $( "#designChangeDetails" ).val() );
        },

        10 ( formData )
        {
            formData.append( "dateaccomplishedmodified", $( "#dateAccomplishedModified" ).val() );
            formData.append( "partmodifieddetails", $( "#partModifiedDetails" ).val() );
        },

        11 ( formData )
        {
            formData.append( "moldresult", $( "#moldResult" ).val() );
            formData.append( "dateaccomplishedresult", $( "#dateAccomplishedResult" ).val() );
        },

        13 ( formData )
        {
            formData.append( "proddatereceived", $( "#productionDateReceived" ).val() );
            formData.append( "prodtesting", $( "#productionDateTesting" ).val() );
        },

        // Combined Result Evaluation + Quality Verification
        14 ( formData )
        {
            formData.append( "productresult", $( "#productResult" ).val() );
            formData.append( "isapproved", $( "#isApproved" ).is( ":checked" ) );

            formData.append( "job_order", $( "#jobOrdertxt" ).val() );
        },
        15 ( formData )
        {
           
            const withDeviation = $( "#withDeviation" ).is( ":checked" );

            

            formData.append( "deviation_id",
                withDeviation ? deviationno : null );

            formData.append( "deviation_details",
                withDeviation ? $( "#deviationDetails" ).val() : null );

            formData.append( "deviation_date",
                withDeviation ? $( "#deviationDate" ).val() : null );


            if ( withDeviation )
            {

                formData.append( "rcWearTear", $( "#wearTearSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcMachineError", $( "#machineErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcDesignError", $( "#designErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcFabricationError", $( "#fabricationErrorSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcEffectOfPrevImprovement", $( "#prevImpvtSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcCustomerRequirement", $( "#customerReqSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "rcDetails", $( "#rootCauseDetailsTxtUpdateRev" ).val() );

                formData.append( "apRepair", $( "#repairSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apAdjustment", $( "#adjustmentSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apRevision", $( "#revisionSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apReplacement", $( "#replacementSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apTrialTesting", $( "#trialTestingSwitchUpdateRev" ).is( ":checked" ) );
                formData.append( "apDetails", $( "#actionPlanDetailsTxtUpdateRev" ).val() );

            }




        }

    }

};


const validationWorkflow = {

    PD: {

        9 ()
        {
            const isRush = $( "#isRush" ).is( ":checked" );
            const isNextProduction = $( "#isNextProduction" ).is( ":checked" );

            if ( !isRush && !isNextProduction )
            {
                showToast( "Must select one: Rush or Next Production." );
                return false;
            }

            if ( !$( "#statusAsOf" ).val() )
            {
                markInvalidField( "#statusAsOf" );
                showToast( "Please select Status As Of date." );
                return false;
            }

            if ( !$( "#requiredDate" ).val() )
            {
                markInvalidField( "#requiredDate" );
                showToast( "Please select Required Date." );
                return false;
            }

            if ( !$( "#statusDetails" ).val().trim() )
            {
                markInvalidField( "#statusDetails" );
                showToast( "Please enter status details." );
                return false;
            }

            return true;
        },

        10 ()
        {
            if ( !$( "#dateAccomplishedChange" ).val() )
            {
                markInvalidField( "#dateAccomplishedChange" );
                showToast( "Please select Date Accomplished Change." );
                return false;
            }

            if ( !$( "#designChangeDetails" ).val().trim() )
            {
                markInvalidField( "#designChangeDetails" );
                showToast( "Please enter design change details." );
                return false;
            }

            return true;
        },

        11 ()
        {
            if ( !$( "#dateAccomplishedModified" ).val() )
            {
                markInvalidField( "#dateAccomplishedModified" );
                showToast( "Please select Date Accomplished Modified." );
                return false;
            }

            if ( !$( "#partModifiedDetails" ).val().trim() )
            {
                markInvalidField( "#partModifiedDetails" );
                showToast( "Please enter part modified details." );
                return false;
            }

            return true;
        },

        12 ()
        {
            if ( !$( "#moldResult" ).val().trim() )
            {
                markInvalidField( "#moldResult" );
                showToast( "Please enter mold result." );
                return false;
            }

            if ( !$( "#dateAccomplishedResult" ).val() )
            {
                markInvalidField( "#dateAccomplishedResult" );
                showToast( "Please select Date Accomplished Result." );
                return false;
            }

            return true;
        },

        14 ()
        {
            if ( !$( "#productionDateTesting" ).val() )
            {
                markInvalidField( "#productionDateTesting" );
                showToast( "Please select Production Testing Date." );
                return false;
            }

            return true;
        },

        15 ()
        {
            const isApproved = $( "#isApproved" ).is( ":checked" );
            const isNotApproved = $( "#isNotApproved" ).is( ":checked" );

            const withDeviation = $( "#withDeviation" ).is( ":checked" );

            if ( withDeviation )
            {
                // Deviation Details
                if ( !$( "#deviationDetails" ).val().trim() )
                {
                    markInvalidField( "#deviationDetails" );
                    showToast( "Please enter deviation details / deviation reason." );
                    return false;
                }

                // Root Cause - At least one switch
                const hasRootCause =
                    $( "#wearTearSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#machineErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#prevImpvtSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#designErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#fabricationErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#customerReqSwitchUpdateRev" ).is( ":checked" );

                if ( !hasRootCause )
                {
                    markInvalidGroup( "#rootCauseGroupUpdateRev" );
                    showToast( "Please select at least one Root Cause." );
                    return false;
                }

                // Root Cause Details
                if ( !$( "#rootCauseDetailsTxtUpdateRev" ).val().trim() )
                {
                    markInvalidField( "#rootCauseDetailsTxtUpdateRev" );
                    showToast( "Please enter Root Cause details." );
                    return false;
                }

                // Action Plan - At least one switch
                const hasActionPlan =
                    $( "#repairSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#adjustmentSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#revisionSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#replacementSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#trialTestingSwitchUpdateRev" ).is( ":checked" );
                if ( !hasActionPlan )
                {
                    markInvalidGroup( "#actionPlanGroupUpdateRev" );
                    showToast( "Please select at least one Proposed Action Plan." );
                    return false;
                }

                // Action Plan Details
                if ( !$( "#actionPlanDetailsTxtUpdateRev" ).val().trim() )
                {
                    markInvalidField( "#actionPlanDetailsTxtUpdateRev" );
                    showToast( "Please enter Proposed Action Plan details." );
                    return false;
                }
            }

            if ( !$( "#productResult" ).val().trim() )
            {
                markInvalidField( "#productResult" );
                showToast( "Please enter product result details." );
                return false;
            }

            if ( !isApproved && !isNotApproved )
            {
                showToast( "Must select one: Approved or Rejected." );
                return false;
            }

            return true;
        },

        16 ()
        {
            if ( !$( "#jobOrdertxt" ).val().trim() )
            {
                markInvalidField( "#jobOrdertxt" );
                showToast( "Please enter Job Order." );
                return false;
            }

            return true;
        }

    },

    MP: {

        8 ()
        {
            return validationWorkflow.PD[ 9 ]();
        },

        9 ()
        {
            return validationWorkflow.PD[ 10 ]();
        },

        10 ()
        {
            return validationWorkflow.PD[ 11 ]();
        },

        11 ()
        {
            return validationWorkflow.PD[ 12 ]();
        },

        13 ()
        {
            return validationWorkflow.PD[ 14 ]();
        },

        14 ()
        {
            const isApproved = $( "#isApproved" ).is( ":checked" );
            const isNotApproved = $( "#isNotApproved" ).is( ":checked" );

            if ( !$( "#productResult" ).val().trim() )
            {
                markInvalidField( "#productResult" );
                showToast( "Please enter product result details." );
                return false;
            }

            if ( !isApproved && !isNotApproved )
            {
                showToast( "Must select one: Approved or Rejected." );
                return false;
            }

            if ( !$( "#jobOrdertxt" ).val().trim() )
            {
                markInvalidField( "#jobOrdertxt" );
                showToast( "Please enter Job Order." );
                return false;
            }

            return true;
        },
        15 ()
        {
            const withDeviation = $( "#withDeviation" ).is( ":checked" );

            if ( withDeviation )
            {
                // Deviation Details
                if ( !$( "#deviationDetails" ).val().trim() )
                {
                    markInvalidField( "#deviationDetails" );
                    showToast( "Please enter deviation details / deviation reason." );
                    return false;
                }

                // Root Cause - At least one switch
                const hasRootCause =
                    $( "#wearTearSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#machineErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#prevImpvtSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#designErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#fabricationErrorSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#customerReqSwitchUpdateRev" ).is( ":checked" );

                if ( !hasRootCause )
                {
                    markInvalidGroup( "#rootCauseGroupUpdateRev" );
                    showToast( "Please select at least one Root Cause." );
                    return false;
                }

                // Root Cause Details
                if ( !$( "#rootCauseDetailsTxtUpdateRev" ).val().trim() )
                {
                    markInvalidField( "#rootCauseDetailsTxtUpdateRev" );
                    showToast( "Please enter Root Cause details." );
                    return false;
                }

                // Action Plan - At least one switch
                const hasActionPlan =
                    $( "#repairSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#adjustmentSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#revisionSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#replacementSwitchUpdateRev" ).is( ":checked" ) ||
                    $( "#trialTestingSwitchUpdateRev" ).is( ":checked" );
                if ( !hasActionPlan )
                {
                    markInvalidGroup( "#actionPlanGroupUpdateRev" );
                    showToast( "Please select at least one Proposed Action Plan." );
                    return false;
                }

                // Action Plan Details
                if ( !$( "#actionPlanDetailsTxtUpdateRev" ).val().trim() )
                {
                    markInvalidField( "#actionPlanDetailsTxtUpdateRev" );
                    showToast( "Please enter Proposed Action Plan details." );
                    return false;
                }
            }

            return true;
        }

    }

};



$( document ).on( "click", "#approveRequestBtn", function ()
{





    if ( !$( "#approvalRemarks" ).val().trim() )
    {
        markInvalidField( "#approvalRemarks" );
        showToast( "Please enter approval remarks." );
        return false;
    }

    const validateStep = validationWorkflow[ currenttype ]?.[ selectedPart ];

    if ( validateStep && !validateStep() )
    {
        return false;
    }


    const withDeviation = $( "#withDeviation" ).is( ":checked" );

    const formData = new FormData();

    formData.append( "trrs_id", selectedTrrsId );
    formData.append( "approving_part", selectedPart );
    formData.append( "approved_date", $( "#approvalDate" ).val() );
    formData.append( "approver_remarks", $( "#approvalRemarks" ).val() );
    formData.append( "current_role", currentRole );
    formData.append( "trrs_type", currenttype );

    formData.append( "withdeviation",
        withDeviation ? 1 : 0 );

    const submitStep = submitWorkflow[ currenttype ]?.[ selectedPart ];

    if ( submitStep )
    {
        submitStep( formData );
    }
   

    const files = $( "#approvalAttachment" )[ 0 ].files;

    for ( let i = 0; i < files.length; i++ )
    {
        formData.append( "files", files[ i ] );
    }

    if ( withDeviation )
    {
        const rootcauseFiles = $( "#rootCauseAttachmentUpdate" )[ 0 ].files;

        for ( let i = 0; i < rootcauseFiles.length; i++ )
        {
            formData.append( "RootCauseAttachments", rootcauseFiles[ i ] );
        }

        const actionplanFiles = $( "#actionPlanAttachmentUpdate" )[ 0 ].files;

        for ( let i = 0; i < actionplanFiles.length; i++ )
        {
            formData.append( "ActionPlanAttachments", actionplanFiles[ i ] );
        }


    }

    $.ajax( {
        url: "/TRRF/ApproveRequest",
        type: "POST",
        data: formData,

        processData: false,
        contentType: false,

        beforeSend: function ()
        {
            $( "#approveRequestBtn" ).prop( "disabled", true );
        },

        success: function ( response )
        {
            if ( response.success )
            {
                showToast( response.message, "success" );

                closeApprovalDrawers();

                if ( window.mainTable )
                {
                    window.mainTable.replaceData();
                }
            }
            else
            {
                showToast( response.message, "error" );
            }
        },

        error: function ()
        {
            showToast( "An unexpected error occurred.", "error" );
        },

        complete: function ()
        {
            $( "#approveRequestBtn" ).prop( "disabled", false );
        }
    } );
} );



$( document ).on( "change", "#withDeviation", function ()
{
    if ( $( this ).is( ":checked" ) )
    {
        $( "#deviationFields" ).removeClass( "hidden" );
    }
    else
    {
        $( "#deviationFields" ).addClass( "hidden" );
    }
} );