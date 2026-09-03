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




function validateSubmitForm ()
{

	const isMpChecked = $( "#mpSwitch" ).is( ":checked" );
	const isPdChecked = $( "#pdSwitch" ).is( ":checked" );

	if ( !isMpChecked && !isPdChecked )
	{
		markInvalidGroup( "#TrrfSwitchdiv" );
		showToast( "Please pick any type of TRRF first." );
		return false;
	}



	const hasRootCauseChecked =
		$( "#wearTearSwitch" ).is( ":checked" ) ||
		$( "#machineErrorSwitch" ).is( ":checked" ) ||
		$( "#designErrorSwitch" ).is( ":checked" ) ||
		$( "#fabricationErrorSwitch" ).is( ":checked" ) ||
		$( "#prevImpvtSwitch" ).is( ":checked" ) ||
		$( "#customerReqSwitch" ).is( ":checked" );

	const hasActionPlanChecked =
		$( "#repairSwitch" ).is( ":checked" ) ||
		$( "#adjustmentSwitch" ).is( ":checked" ) ||
		$( "#revisionSwitch" ).is( ":checked" ) ||
		$( "#replacementSwitch" ).is( ":checked" ) ||
		$( "#trialTestingSwitch" ).is( ":checked" );

	const hasSpareValue = $( "input[name='hasSpare']:checked" ).val();

	if ( !parseInt( $( "#customerIdTxt" ).val() || 0 ) )
	{
		markInvalidField( "#customerIdTxt" );
		showToast( "Please select a customer." );
		return false;
	}

	if ( !parseInt( $( "#productIdTxt" ).val() || 0 ) )
	{
		markInvalidField( "#productNameTxt" );
		showToast( "Please select or add a product." );
		return false;
	}

	if ( !parseInt( $( "#moldNoTxt" ).val() || 0 ) )
	{
		markInvalidField( "#moldNoTxt" );
		showToast( "Please enter mold number." );
		return false;
	}

	if ( !$( "#partModelNoTxt" ).val().trim() )
	{
		markInvalidField( "#partModelNoTxt" );
		showToast( "Please enter part/model number." );
		return false;
	}

	if ( !$( "#encounteredProblemTxt" ).val().trim() )
	{
		markInvalidField( "#encounteredProblemTxt" );
		showToast( "Please enter encountered problem." );
		return false;
	}

	if ( !$( "#machineNameTxt" ).val().trim() )
	{
		markInvalidField( "#machineNameTxt" );
		showToast( "Please enter machine name." );
		return false;
	}

	if ( !$( "#moldToolLifeTxt" ).val().trim() )
	{
		markInvalidField( "#moldToolLifeTxt" );
		showToast( "Please enter mold tool life." );
		return false;
	}

	if ( !hasRootCauseChecked )
	{
		markInvalidGroup( "#rootCauseGroup" );
		showToast( "Please select at least one root cause." );
		return false;
	}

	if ( !$( "#rootCauseDetailsTxt" ).val().trim() )
	{
		markInvalidField( "#rootCauseDetailsTxt" );
		showToast( "Please enter root cause details." );
		return false;
	}

	if ( !hasActionPlanChecked )
	{
		markInvalidGroup( "#actionPlanGroup" );
		showToast( "Please select at least one action plan." );
		return false;
	}

	if ( !$( "#actionPlanDetailsTxt" ).val().trim() )
	{
		markInvalidField( "#actionPlanDetailsTxt" );
		showToast( "Please enter action plan details." );
		return false;
	}

	if ( !hasSpareValue )
	{
		markInvalidGroup( "#hasSpareGroup" );
		showToast( "Please select Yes or No for spare." );
		return false;
	}

	if ( hasSpareValue === "Yes" && !parseInt( $( "#spareQuantityTxt" ).val() || 0 ) )
	{
		markInvalidField( "#spareQuantityTxt" );
		showToast( "Please enter spare quantity." );
		return false;
	}

	return true;
}

















$( document ).on( "click", "#submitTrrfBtn", function ( e )
{
	console.log( "SUBMIT CLICKED" );
	e.preventDefault();
	e.stopPropagation();

	if ( !validateSubmitForm() )
	{
		return;
	}

	const hasSpareValue = $( "input[name='hasSpare']:checked" ).val();

	const formData = new FormData();

	formData.append( "Type", $( "#pdSwitch" ).is( ":checked" ) ? "PD" : "MP" );

	formData.append( "CustomerId", $( "#customerIdTxt" ).val() );
	formData.append( "ProductId", $( "#productIdTxt" ).val() );
	formData.append( "MoldNo", $( "#moldNoTxt" ).val() );
	formData.append( "PartModelNo", $( "#partModelNoTxt" ).val() );
	formData.append( "DatePrepared", $( "#datePreparedTxt" ).val() );

	formData.append( "EncounteredProblem", $( "#encounteredProblemTxt" ).val() );
	formData.append( "MachineName", $( "#machineNameTxt" ).val() );
	formData.append( "MoldToolLife", $( "#moldToolLifeTxt" ).val() );

	formData.append( "RcWearTear", $( "#wearTearSwitch" ).is( ":checked" ) );
	formData.append( "RcMachineError", $( "#machineErrorSwitch" ).is( ":checked" ) );
	formData.append( "RcDesignError", $( "#designErrorSwitch" ).is( ":checked" ) );
	formData.append( "RcFabricationError", $( "#fabricationErrorSwitch" ).is( ":checked" ) );
	formData.append( "RcEffectOfPrevImprovement", $( "#prevImpvtSwitch" ).is( ":checked" ) );
	formData.append( "RcCustomerRequirement", $( "#customerReqSwitch" ).is( ":checked" ) );
	formData.append( "RcDetails", $( "#rootCauseDetailsTxt" ).val() );

	formData.append( "ApRepair", $( "#repairSwitch" ).is( ":checked" ) );
	formData.append( "ApAdjustment", $( "#adjustmentSwitch" ).is( ":checked" ) );
	formData.append( "ApRevision", $( "#revisionSwitch" ).is( ":checked" ) );
	formData.append( "ApReplacement", $( "#replacementSwitch" ).is( ":checked" ) );
	formData.append( "ApTrialTesting", $( "#trialTestingSwitch" ).is( ":checked" ) );
	formData.append( "ApDetails", $( "#actionPlanDetailsTxt" ).val() );
	formData.append( "MeetingMinutesRemarksTxt", $( "#meetingMinutesRemarksTxt" ).val() );

	formData.append( "HasSpare", hasSpareValue === "Yes" );
	formData.append( "Quantity",
		hasSpareValue === "No"
			? 0
			: parseInt( $( "#spareQuantityTxt" ).val() || 0 ) );

	formData.append( "productname", $( "#productNameTxt" ).val() );

	const rootCauseFiles = $( "#rootCauseAttachment" )[ 0 ].files;

	for ( let i = 0; i < rootCauseFiles.length; i++ )
	{
		formData.append( "RootCauseAttachments", rootCauseFiles[ i ] );
	}

	const actionPlanFiles = $( "#actionPlanAttachment" )[ 0 ].files;

	for ( let i = 0; i < actionPlanFiles.length; i++ )
	{
		formData.append( "ActionPlanAttachments", actionPlanFiles[ i ] );
	}


	const momPlanFiles = $( "#meetingMinutesAttachment" )[ 0 ].files;

  	for ( let i = 0; i < momPlanFiles.length; i++ )
	{
		formData.append( "MeetingMinutesAttachments", momPlanFiles[ i ] );
	}	

	$.ajax( {
		url: $( "#submitTrrfForm" ).data( "insert-url" ),
		type: "POST",
		data: formData,
		processData: false,
		contentType: false,
		success: function ( res )
		{
			showToast( res.message, "success" );
			$( "#submitTrrfForm" )[ 0 ].reset();
			closeSubmitDrawer();

			// Refresh Tabulator data
			if ( window.mainTable )
			{
				window.mainTable.replaceData();
			}
		},
		error: function ( xhr )
		{
			console.error( xhr.responseText );
			showToast( "Failed to submit TRRF." );
		}
	} );
} );