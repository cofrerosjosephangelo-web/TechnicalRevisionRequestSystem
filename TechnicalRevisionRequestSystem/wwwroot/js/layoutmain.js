document.addEventListener( "DOMContentLoaded", function ()
{
	const sidebar = document.getElementById( "sidebar" );
	const toggle = document.getElementById( "sidebarToggle" );

	if ( !sidebar || !toggle ) return;

	// ==========================================
	// RESTORE SIDEBAR STATE
	// ==========================================
	let collapsed = localStorage.getItem( "sidebarCollapsed" ) === "true";

	function applySidebarState ()
	{
		if ( collapsed )
		{
			sidebar.classList.remove( "w-64" );
			sidebar.classList.add( "w-20" );

			document.querySelectorAll( ".menu-text" ).forEach( el =>
			{
				el.style.display = "none";
			} );
		}
		else
		{
			sidebar.classList.remove( "w-20" );
			sidebar.classList.add( "w-64" );

			document.querySelectorAll( ".menu-text" ).forEach( el =>
			{
				el.style.display = "inline";
			} );
		}
	}

	// Apply saved state immediately on page load
	applySidebarState();

	// ==========================================
	// TOGGLE SIDEBAR
	// ==========================================
	toggle.addEventListener( "click", function ()
	{
		collapsed = !collapsed;

		// Save state
		localStorage.setItem( "sidebarCollapsed", collapsed );

		// Apply state
		applySidebarState();

		// Redraw table after sidebar animation
		setTimeout( function ()
		{
			if ( window.mainTable )
			{
				window.mainTable.redraw( true );
			}
		}, 350 );
	} );
} );





function showToast ( message, type = "error" )
{
	const bgClass = type === "success"
		? "bg-emerald-600"
		: "bg-red-600";

	const toast = $( `
			<div class="${ bgClass } text-white px-4 py-3 rounded-lg shadow-lg text-sm font-medium min-w-72 max-w-md">
				${ message }
			</div>
		`);

	$( "#toastContainer" ).append( toast );

	setTimeout( function ()
	{
		toast.fadeOut( 250, function ()
		{
			$( this ).remove();
		} );
	}, 3000 );
}


const profileBtn = document.getElementById( "profileMenuBtn" );
const profileMenu = document.getElementById( "profileDropdown" );
const profileArrow = document.getElementById( "profileArrow" );

if ( profileBtn )
{

	profileBtn.addEventListener( "click", function ( e )
	{

		e.stopPropagation();

		profileMenu.classList.toggle( "hidden" );
		profileArrow.classList.toggle( "rotate-180" );

	} );

	document.addEventListener( "click", function ( e )
	{

		if ( !profileMenu.contains( e.target ) && !profileBtn.contains( e.target ) )
		{

			profileMenu.classList.add( "hidden" );
			profileArrow.classList.remove( "rotate-180" );

		}

	} );

}

$( "#btnExportExcel" ).click( function ()
{

	$( "#excelExportOverlay" )
		.removeClass( "hidden" )
		.addClass( "flex" );

	// Create hidden iframe once
	let iframe = $( "#downloadFrame" );

	if ( iframe.length === 0 )
	{

		$( "body" ).append( `
            <iframe id="downloadFrame"
                    style="display:none;"></iframe>
        `);

		iframe = $( "#downloadFrame" );
	}

	iframe.attr( "src", "/TRRF/ExportExcel" );

	// Hide overlay after a few seconds
	// (or when download cookie is detected - see below)
	setTimeout( function ()
	{

		$( "#excelExportOverlay" )
			.removeClass( "flex" )
			.addClass( "hidden" );

	}, 2000 );

} );


function refreshDashboard ()
{
	loadDashboardSummary();
	loadMonthlyChart();
	loadStatusChart();
	loadCustomerChart();
	loadProductChart();
	loadRevisionChart();
	loadMoldChart();
	loadDeviationChart();
	loadTopRequestedProductsTable();
	loadMonthlyTrendTable();
	loadDetailedBreakdownTable();
}





//$( "#btnExportExcel" ).on( "click", function ()
//{

//	window.mainTable.download( "xlsx", "TRRS_Report.xlsx", {
//		sheetName: "TRRS Report",

//		documentProcessing: function ( workbook )
//		{

//			const sheet = workbook.Sheets[ "TRRS Report" ];

//			if ( !sheet || !sheet[ "!ref" ] )
//			{
//				return workbook;
//			}

//			const range = XLSX.utils.decode_range( sheet[ "!ref" ] );

//			const colWidths = [];

//			// Loop through all cells
//			for ( let R = range.s.r; R <= range.e.r; R++ )
//			{

//				for ( let C = range.s.c; C <= range.e.c; C++ )
//				{

//					const address = XLSX.utils.encode_cell( { r: R, c: C } );
//					const cell = sheet[ address ];

//					if ( !cell ) continue;

//					// ----------------------------
//					// Calculate Auto Width
//					// ----------------------------
//					const text = cell.v == null ? "" : cell.v.toString();

//					colWidths[ C ] = Math.max(
//						colWidths[ C ] || 10,
//						text.length + 3
//					);

//					// ----------------------------
//					// Cell Style
//					// ----------------------------
//					cell.s = {
//						border: {
//							top: { style: "thin", color: { rgb: "000000" } },
//							bottom: { style: "thin", color: { rgb: "000000" } },
//							left: { style: "thin", color: { rgb: "000000" } },
//							right: { style: "thin", color: { rgb: "000000" } }
//						},
//						alignment: {
//							vertical: "center",
//							horizontal: "left",
//							wrapText: true
//						}
//					};

//					// Header Row
//					if ( R === 0 )
//					{

//						cell.s.font = {
//							bold: true,
//							color: { rgb: "FFFFFF" }
//						};

//						cell.s.fill = {
//							patternType: "solid",
//							fgColor: { rgb: "1F4E78" }
//						};

//						cell.s.alignment.horizontal = "center";
//					}
//				}
//			}

//			// Apply Auto Width
//			sheet[ "!cols" ] = colWidths.map( w => ( {
//				wch: Math.min( w, 60 )
//			} ) );

//			return workbook;
//		}
//	} );

//} );