

$( "#myApprovalsOnly" ).on( "change", function ()
{

    const url = this.checked
        ? "/TRRF/GetMyApprovalRequestList"
        : "/TRRF/GetRequestList";

    mainTable.setData( url );

} );





function closeApprovalDrawers ()
{
    document
        .getElementById( "leftApprovalDrawer" )
        .classList.add( "-translate-x-full" );

    document
        .getElementById( "rightApprovalDrawer" )
        .classList.add( "translate-x-full" );

    document
        .getElementById( "approvalQuickNav" )
        .classList.add( "hidden" );

    setTimeout( () =>
    {
        document
            .getElementById( "approvalOverlay" )
            .classList.add( "hidden" );
    }, 300 );
}

document
    .getElementById( "closeApprovalDrawer" )
    ?.addEventListener( "click", closeApprovalDrawers );

document
    .getElementById( "cancelApprovalDrawer" )
    ?.addEventListener( "click", closeApprovalDrawers );

document
    .getElementById( "approvalOverlay" )
    ?.addEventListener( "click", closeApprovalDrawers );



const workflows = {
    PD: [
        {
            part: 9,
            view: "#viewProductStatusCard",
            edit: "#productStatusSection",
            loader: loadProductStatus
        },
        {
            part: 10,
            view: "#viewActualActivityCard",
            edit: "#actualinfodesign",
            loader: loadActualInfo
        },
        {
            part: 11,
            view: "#viewActualActivityCard",
            edit: "#actualinfomodified",
            loader: loadActualInfo
        },
        {
            part: 12,
            view: "#viewActualActivityCard",
            edit: "#actualinforesult",
            loader: loadActualInfo
        },
        {
            part: 14,
            view: "#viewProductionScheduleCard",
            edit: "#productionschedulepanel",
            loader: loadProductionSchedule
        },
        {
            part: 15,
            view: "#viewProductResultCard",
            edit: "#productresult",
            loader: loadProductResult
        },
        {
            part: 15,
            view: "",
            edit: "#withdeviation",
            loader: ""
        },
        {
            part: 16,
            view: "#joborderpanel",
            edit: "#jobOrderPanel",
            loader: loadQualityVerification
        }
    ],

    MP: [
        {
            part: 8,
            view: "#viewProductStatusCard",
            edit: "#productStatusSection",
            loader: loadProductStatus
        },
        {
            part: 9,
            view: "#viewActualActivityCard",
            edit: "#actualinfodesign",
            loader: loadActualInfo
        },
        {
            part: 10,
            view: "#viewActualActivityCard",
            edit: "#actualinfomodified",
            loader: loadActualInfo
        },
        {
            part: 11,
            view: "#viewActualActivityCard",
            edit: "#actualinforesult",
            loader: loadActualInfo
        },
        {
            part: 13,
            view: "#viewProductionScheduleCard",
            edit: "#productionschedulepanel",
            loader: loadProductionSchedule
        },
        {
            // Result Evaluation + Quality Verification
            part: 14,
            view: "#viewProductResultCard",
            edit: "#productresult",
            loader: loadProductResult
        },

        // Quality Verification (same workflow step)
        {
            part: 14,
            view: "#joborderpanel",
            edit: "#jobOrderPanel",
            loader: loadQualityVerification
        },
        {
            part: 15,
            view: "",
            edit: "#withdeviation",
            loader: ""
        },
        {
            part: 16
        },
        {
            part: 17
        }
    ]
};

window.mainTable = new Tabulator( "#trrs-table", {
    layout: "fitDataTable",
    responsiveLayout: false,
    height: "calc(100vh - 210px)",
    pagination: true,
    paginationMode: "remote",
    variableHeight: true,

    ajaxURL: "/TRRF/GetRequestList",

    ajaxResponse: function ( url, params, response )
    {
        return response;
    },
    columnDefaults: {
        headerSort: false,
        headerMouseEnter: function ( e )
        {
            e.target.style.cursor = "pointer";
        }

    },

    columns: [

        // =========================
        // EXPAND BUTTON COLUMN
        // =========================
        {
            formatter: function ()
            {
                return "<span class='toggle-child'><i class='fa-solid fa-caret-right'></i></span>";
            },
            width: 40,
            hozAlign: "center",
            frozen: true,
            headerSort: false,

            cellClick: function ( e, cell )
            {
                const row = cell.getRow();
                const rowEl = row.getElement();

                const existing = rowEl.nextElementSibling;

                // -------------------------
                // COLLAPSE
                // -------------------------
                if ( existing && existing.classList.contains( "child-row" ) )
                {
                    existing.remove();
                    cell.getElement().innerHTML = '<i class="fa-solid fa-caret-right"></i>';
                    return;
                }

                // -------------------------
                // EXPAND ICON
                // -------------------------
                cell.getElement().innerHTML = '<i class="fa-solid fa-caret-down"></i>';

                const data = row.getData();

                const childRow = document.createElement( "div" );
                childRow.classList.add( "child-row" );

                childRow.style.padding = "10px";
                childRow.style.background = "#fafafa";
                childRow.style.borderTop = "1px solid #e5e7eb";

                const childTable = document.createElement( "div" );
                childRow.appendChild( childTable );

                rowEl.after( childRow );

                new Tabulator( childTable, {
                    layout: "fitDataStretch",

                    ajaxURL: "/TRRF/GetRequestApprovals",
                    ajaxParams: {
                        trrsId: data.id
                    },

                    ajaxResponse: function ( url, params, response )
                    {
                        return response.data;
                    },



                    rowFormatter: function ( row )
                    {
                        const data = row.getData();
                        const el = row.getElement();

                        const table = row.getTable();
                        const rows = table.getRows();

                        // Find the first deviated row
                        const deviationIndex = rows.findIndex( r => r.getData().done == 3 );

                        // Current row index
                        const currentIndex = row.getPosition( true ) - 1;

                        if ( deviationIndex !== -1 && currentIndex > deviationIndex )
                        {
                            el.classList.add( "child-row-muted" );
                        }

                        if ( data.done == 1 )
                        {
                            el.classList.add( "child-row-approved" );
                        }
                        else
                        {
                            el.classList.add( "child-row-pending" );
                        }
                    },

                    columns: [
                        {
                            title: "",
                            width: 110,
                            hozAlign: "center",
                            headerSort: false,

                            formatter: function ( cell )
                            {
                                const data = cell.getRow().getData();

                                const deptMatch = data.deptid == userDeptId;
                                const sectionId = parseInt(data.sectionid);
                                const forApprove = data.forapprove == 1;
                                const clickedRole = data.role;

                                let sectionMatch = false;

                                if ( sectionId === 1 )
                                {
                                    // Section 1 can only be approved by Section 1
                                    sectionMatch = userSectionId === 1;
                                }
                                else if (
                                    sectionId === 2 &&
                                    (
                                        clickedRole === "PRODUCT STATUS INFORMATION" ||
                                        clickedRole === "TRIAL / PROD SCHEDULE"
                                    )
                                )
                                {
                                    // Section 2 can be approved by Section 2 or 4
                                    sectionMatch = userSectionId === 2 || userSectionId === 4;
                                }
                                else
                                {
                                    // All other sections must match exactly
                                    sectionMatch = sectionId === userSectionId;
                                }

                                const canApprove =
                                    deptMatch &&
                                    sectionMatch &&
                                    forApprove;

                                // already approved → show badge instead of button
                                if ( data.done == 1 )
                                {
                                    return `<span class="text-green-600 text-xs font-semibold">
                            <i class="fa-solid fa-circle-check me-1"></i> Done
                        </span>`;
                                }

                                if ( data.done == 3 )
                                {
                                    return `<span class="text-blue-600 text-xs font-semibold">
                            <i class="fa-solid fa-rotate me-1"></i> Revised
                        </span>`;
                                }

                                if (canApprove)
                                {
                                    return `
                    <button class="approve-btn inline-flex items-center gap-1 px-2 py-1 rounded bg-green-800 text-white text-xs hover:bg-green-700 transition">
                        <i class="fa-solid fa-check"></i> Approve
                    </button>
                `;
                                }

                                return `<span class="text-gray-300 text-xs">—</span>`;
                            },

                            cellClick: function ( e, cell )
                            {
                                const data = cell.getRow().getData();

                                selectedTrrsId = data.id;
                                currentTrrsId = data.id;
                                selectedforapprove = data.forapprove;
                                selectedPart = data.part;
                                currentRole = data.role;
                                currenttype = data.trrstype;
								deviationno = data.deviationid;

                                loadRequestDetails( selectedTrrsId );
                                loadApprovalHistory( selectedTrrsId, selectedPart );

                                // Hide all panels first
                                [
                                    "#viewProductStatusCard",
                                    "#viewActualActivityCard",
                                    "#viewProductionScheduleCard",
                                    "#viewProductResultCard",
                                    "#joborderpanel",

                                    "#productStatusSection",
                                    "#actualinfodesign",
                                    "#actualinfomodified",
                                    "#actualinforesult",
                                    "#productionschedulepanel",
                                    "#productresult",
                                    "#jobOrderPanel"
                                ].forEach( selector => $( selector ).addClass( "hidden" ) );

                                const workflow = workflows[ currenttype ] || [];

                                workflow.forEach( step =>
                                {

                                    if ( selectedPart > step.part )
                                    {

                                        if ( step.view )
                                        {
                                            $( step.view ).removeClass( "hidden" );
                                        }

                                        if ( typeof step.loader === "function" )
                                        {
                                            step.loader === loadProductResult
                                                ? step.loader( selectedTrrsId, currenttype )
                                                : step.loader( selectedTrrsId );
                                        }
                                    }

                                    if ( selectedPart === step.part && step.edit )
                                    {
                                        $( step.edit ).removeClass( "hidden" );
                                    }
                                } );

                                $( "#approvalOverlay" ).removeClass( "hidden" );

                                $( "#leftApprovalDrawer" )
                                    .removeClass( "-translate-x-full" );

                                $( "#rightApprovalDrawer" )
                                    .removeClass( "translate-x-full" );
                            }
                        },
                        { title: '<i class="fa-solid fa-puzzle-piece me-1"></i> Part', field: "part", width: 100, hozAlign: "center", headerHozAlign: "center" },
                        { title: '<i class="fa-solid fa-user-tag me-1"></i> Role', field: "role", width: 350 },

                        {
                            title: '<i class="fa-solid fa-circle-check me-1"></i> Status',
                            field: "done",
                            hozAlign: "center",
                            headerHozAlign: "center",
                            width: 120,
                            formatter: function ( cell )
                            {
                                const el = cell.getElement();
                                const value = cell.getValue();

                                el.classList.remove( "done-yes", "done-no" );

                                if ( value == 1 )
                                {
                                    el.classList.add( "done-yes" );
                                    return "Approved";
                                }

                                if ( value == 3 )
                                {
                                    el.classList.add( "done-dev" );
                                    return "Revised";
                                }

                                el.classList.add( "done-no" );
                                return "Pending";
                            }
                        },

                        { title: '<i class="fa-solid fa-user-check me-1"></i> Approved By', field: "approvedby", width: 250, hozAlign: "center", headerHozAlign: "center" },

                        {
                            title: '<i class="fa-solid fa-calendar-check me-1"></i> Approved Date',
                            hozAlign: "center",
                            headerHozAlign: "center",
                            field: "approveddate",
                            formatter: function ( cell )
                            {
                                const value = cell.getValue();
                                if ( !value ) return "";

                                const date = new Date( value );

                                const mm = String( date.getMonth() + 1 ).padStart( 2, "0" );
                                const dd = String( date.getDate() ).padStart( 2, "0" );
                                const yyyy = date.getFullYear();

                                return `${ mm }-${ dd }-${ yyyy }`;
                            }
                        },
                        {
                            title: "<i class='fa-solid fa-note-sticky me-1'></i> Notes",
                            width: 250,
                            hozAlign: "center",
                            headerHozAlign: "center",
                            headerSort: false,

                            formatter: function ( cell )
                            {
                                const data = cell.getRow().getData();

                                // ==========================================
                                // NOTES
                                // ==========================================
                                const notes = data.notes
                                    ? String( data.notes ).trim()
                                    : "";

                                // ==========================================
                                // VIEW NOTES
                                // ------------------------------------------
                                // View Notes does NOT use:
                                // deptMatch
                                // sectionMatch
                                // forApprove
                                //
                                // If notes exist → show View Notes
                                // ==========================================
                                if ( notes )
                                {
                                    return `
                <button
                    class="view-notes-btn inline-flex items-center gap-1
                           px-2 py-1 rounded bg-slate-600 text-white text-xs
                           hover:bg-slate-500 transition">
                    <i class="fa-solid fa-eye"></i>
                    View Notes
                </button>
            `;
                                }

                                // ==========================================
                                // ADD NOTES ACCESS FILTER
                                // ==========================================
                                const deptMatch = data.deptid == userDeptId;
                                const sectionId = parseInt( data.sectionid );
                                const forApprove = data.forapprove == 1;
                                const clickedRole = data.role;

                                let sectionMatch = false;

                                if ( sectionId === 1 )
                                {
                                    sectionMatch = userSectionId === 1;
                                }
                                else if (
                                    sectionId === 2 &&
                                    (
                                        clickedRole === "PRODUCT STATUS INFORMATION" ||
                                        clickedRole === "TRIAL / PROD SCHEDULE"
                                    )
                                )
                                {
                                    sectionMatch = userSectionId === 2 || userSectionId === 4;
                                }
                                else
                                {
                                    sectionMatch = sectionId === userSectionId;
                                }

                                const canAddNotes =
                                    deptMatch &&
                                    sectionMatch &&
                                    forApprove;

                                // ==========================================
                                // NOT AUTHORIZED TO ADD NOTES
                                // ==========================================
                                if ( !canAddNotes )
                                {
                                    return `<span class="text-gray-300 text-xs">—</span>`;
                                }

                                // ==========================================
                                // NO NOTES + AUTHORIZED
                                // → ADD NOTES
                                // ==========================================
                                return `
            <button
                class="add-notes-btn inline-flex items-center gap-1
                       px-2 py-1 rounded bg-blue-600 text-white text-xs
                       hover:bg-blue-500 transition">
                <i class="fa-solid fa-plus"></i>
                Add Notes
            </button>
        `;
                            },

                            // ==========================================
                            // BUTTON CLICK
                            // ==========================================
                            cellClick: function ( e, cell )
                            {
                                const button = $( e.target ).closest(
                                    ".add-notes-btn, .view-notes-btn"
                                );

                                if ( !button.length )
                                {
                                    return;
                                }

                                const data = cell.getRow().getData();

                                console.log( "Notes button clicked:", data );
                                console.log(
                                    "TRRS ID:",
                                    data.trrsid || data.trrsId || data.id
                                );
                                console.log( "Approval ID:", data.approvalId );
                                console.log( "Notes:", data.notes );

                                // ==========================================
                                // VIEW NOTES
                                // ==========================================
                                if ( button.hasClass( "view-notes-btn" ) )
                                {
                                    openViewNotesDrawer( data );
                                    return;
                                }

                                // ==========================================
                                // ADD NOTES
                                // ==========================================
                                if ( button.hasClass( "add-notes-btn" ) )
                                {
                                    openNotesDrawer( data );
                                    return;
                                }
                            }
                        },

                        { title: '<i class="fa-solid fa-comment-dots me-1"></i> Remarks', field: "remarks" },
                        { title: "For Approve", field: "forapprove", width: 80, visible: false },
                        { title: "Approval ID", field: "approvalId", width: 80, visible: false },  
                        { title: "Notes", field: "notes", width: 80, visible: false }, 
                        { title: "Dept", field: "deptid", width: 80 , visible: false },
                        { title: "Section", field: "sectionid", width: 80, visible: false },
                        { title: "Type", field: "trrstype", width: 80, visible: false },
                        { title: "Deviation", field: "deviationid", width: 80, visible: false }
                    ]
                } );
            }
        },

        // =========================
        // HIDDEN ID (IMPORTANT)
        // =========================
        

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-hashtag me-1"></i>
        Control No
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "controlno",

            width: 160,
            frozen: true,

            titleDownload: "Control No",

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-hashtag me-1"></i>
        TRRF Code
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,
            field: "trrfno",
			titleDownload: "TRRF No",
            width: 155,
            frozen: true,
            hozAlign: "center",
            headerHozAlign: "center",

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            },

            formatter: function ( cell )
            {

                const value = cell.getValue();

                if ( value && value !== "N/A" )
                {
                    return `
                <button class="trrf-btn px-2 py-1 rounded bg-blue-600 text-white hover:bg-blue-700 transition">
                    ${ value }
                </button>
            `;
                }

                return value;
            },

            cellClick: function ( e, cell )
            {

                const value = cell.getValue();

                if ( value === "N/A" )
                {
                    return;
                }

                const rowData = cell.getRow().getData();


                // Save the hidden ID
                selectedTrrsId = rowData.id;
                selectedPart = rowData.currentpart;
                deviationId = rowData.deviationid;

                clearViewInformation();

                loadRequestDetails( selectedTrrsId );

                loadRevisionHistory( selectedTrrsId, deviationId );

                loadApprovalHistory( selectedTrrsId, selectedPart );

                $( "#viewProductStatusCard" ).removeClass( "hidden" );
                loadProductStatus( selectedTrrsId );


                $( "#viewActualActivityCard" ).removeClass( "hidden" );
                loadActualInfo( selectedTrrsId );

                $( "#viewProductionScheduleCard" ).removeClass( "hidden" );
                loadProductionSchedule( selectedTrrsId );

                $( "#viewProductResultCard" ).removeClass( "hidden" );
                loadProductResult( selectedTrrsId );

                $( "#joborderpanel" ).removeClass( "hidden" );
                loadQualityVerification( selectedTrrsId );


                $( "#approvalOverlay" ).removeClass( "hidden" );

                $( "#leftApprovalDrawer" )
                    .removeClass( "-translate-x-full" );

                $( "#approvalQuickNav" )
                    .removeClass( "hidden" );
                // Your code here
                // openTRRF(selectedTrrfId);
            }
        },
        
        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-code-branch me-1"></i>
        Version
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "revisionno",

            width: 135,

            titleDownload: "Version",

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },
        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-tag me-1"></i>
        Type
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "trrstype",

            width: 130,

            titleDownload: "Type",

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-building me-1"></i>
        Customer
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "customername",

            width: 180,

            titleDownload: "Customer",

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-box me-1"></i>
        Product
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "productname",

            width: 180,
            frozen: false,

            titleDownload: "Product",
            formatter: function ( cell )
            {
                return `<div style="
            white-space: normal;
            word-break: break-word;
            line-height: 1.3;
        ">${ cell.getValue() || "" }</div>`;
            },

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-building me-1"></i>
        Part Model
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "partmodelno",

            width: 250,

            titleDownload: "Part Model",
            formatter: function ( cell )
            {
                return `<div style="
            white-space: normal;
            word-break: break-word;
            line-height: 1.3;
        ">${ cell.getValue() || "" }</div>`;
            },

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-cube me-1"></i>
        Mold No
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "moldno",

            width: 145,

            titleDownload: "Mold No",
           

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
        <div class="tabulator-header-title">
            <span>
                <i class="fa-solid fa-triangle-exclamation me-1"></i>
                Problem
            </span>
            <i class="fa-solid fa-filter filter-icon"></i>
        </div>
    `,

            field: "problem",

            width: 250,

            variableHeight: true,

            formatter: function ( cell )
            {
                return cell.getValue() || "";
            },

            titleDownload: "Problem",

            headerClick: function ( e, column )
            {
                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );
            }
        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-gear me-1"></i>
        Machine Name
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "machinename",

            width: 200,

            titleDownload: "Machine Name",


            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },
        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-shapes me-1"></i>
        Mold Reference
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "moldreference",

            width: 280,

            titleDownload: "Mold Reference",


            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-user me-1"></i>
        Submitted By
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "submittedby",

            width: 200,

            titleDownload: "Submitted By",


            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        {
            title: `
<div class="tabulator-header-title">
    <span>
        <i class="fa-solid fa-circle-check me-1"></i>
        Current Status
    </span>
    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "currentstatus",


            width: 250,

            titleDownload: "Current Status",


            headerClick: function ( e, column )
            {

                e.stopPropagation();

                // Same column clicked again? Close it.
                if (
                    ExcelFilter.activePopup &&
                    ExcelFilter.activeColumn === column.getField()
                )
                {
                    ExcelFilter.close();
                    return;
                }

                ExcelFilter.open( column, column.getTable() );

            }

        },

        // =========================
        // DATE FILTER (FIXED)
        // =========================
        {
            title: `
<div style="display:flex; align-items:center; justify-content:space-between; width:100%;">
    <span>
        <i class="fa-solid fa-calendar-days me-1"></i>
        Date Prepared
    </span>

    <i class="fa-solid fa-filter filter-icon"></i>
</div>
`,

            field: "dateprepared",

            width: 170,

            titleDownload: "Date Prepared",

            headerSort: false,

            headerClick: function ( e, column )
            {

                e.stopPropagation();

                ExcelFilter.open( column, column.getTable() );

            },

            formatter: function ( cell )
            {

                const value = cell.getValue();

                if ( !value )
                    return "";

                const date = new Date( value );

                return `${ String( date.getMonth() + 1 ).padStart( 2, "0" ) }-${ String( date.getDate() ).padStart( 2, "0" ) }-${ date.getFullYear() }`;

            }
        },
        {
            field: "id",
            visible: false
        }
        ,
        {
            field: "currentpart",
            visible: false
        }
        ,
        {
            field: "deviationid",
            visible: false
        },

    ]
} );


function clearViewInformation ()
{

    // ============================
    // Product Status
    // ============================
    $( "#viewProductStatusCard" ).addClass( "hidden" );

    $( "#viewProductionType" ).text( "-" );
    $( "#viewStatusAsOf" ).text( "-" );
    $( "#viewRequiredDate" ).text( "-" );
    $( "#viewStatusDetails" ).text( "-" );

    // ============================
    // Actual Activity
    // ============================
    $( "#viewActualActivityCard" ).addClass( "hidden" );

    $( "#viewDateReceived" ).text( "-" );
    $( "#viewDateAccomplishedChange" ).text( "-" );
    $( "#viewDesignChangeDetails" ).text( "-" );
    $( "#viewDateAccomplishedModified" ).text( "-" );
    $( "#viewPartModifiedDetails" ).text( "-" );
    $( "#viewDateAccomplishedResult" ).text( "-" );
    $( "#viewMoldResult" ).text( "-" );

    // ============================
    // Production Schedule
    // ============================
    $( "#viewProductionScheduleCard" ).addClass( "hidden" );

    $( "#viewProductionDateReceived" ).text( "-" );
    $( "#viewProductionDateTesting" ).text( "-" );

    // ============================
    // Product Result
    // ============================
    $( "#viewProductResultCard" ).addClass( "hidden" );

    $( "#viewEvaluationResult" ).text( "-" );
    $( "#viewJobOrder" ).text( "-" );
    $( "#viewProductResult" ).text( "-" );

    $( "#joborderpanel" ).addClass( "hidden" );

    // ============================
    // Histories
    // ============================
    $( "#revisionHistoryContainer" ).empty();
    $( "#approvalHistoryContainer" ).empty();
    $( "#approvalQuickNavItems" ).empty();
}



function openNotesDrawer ( data )
{
    const trrsId = data.trrsid || data.trrsId || data.id;
    const approvalId = data.approvalId;

    $( "#notesDrawer" ).data( "trrsId", trrsId );
    $( "#notesDrawer" ).data( "approvalId", approvalId );

    $( "#approvalNotes" ).val( "" );

    updateNotesCharacterCount();

    $( "#notesDrawer" ).removeClass( "invisible" );

    setTimeout( function ()
    {
        $( "#notesDrawerBackdrop" )
            .removeClass( "opacity-0" )
            .addClass( "opacity-100" );

        $( "#notesDrawerPanel" )
            .removeClass( "translate-x-full" )
            .addClass( "translate-x-0" );

        $( "#approvalNotes" ).trigger( "focus" );

    }, 10 );
}

function openViewNotesDrawer ( data )
{
    const notes = data.notes
        ? String( data.notes )
        : "";

    // ==========================================
    // LOAD NOTES
    // ==========================================
    $( "#viewApprovalNotes" ).val( notes );

    // ==========================================
    // CHARACTER COUNT
    // ==========================================
    $( "#viewNotesCharacterCount" ).text(
        `${ notes.length } / 2000`
    );

    // ==========================================
    // OPEN DRAWER
    // ==========================================
    $( "#viewNotesDrawer" )
        .removeClass( "translate-x-full" );

    $( "#viewNotesOverlay" )
        .removeClass( "hidden" );
}

function closeViewNotesDrawer ()
{
    $( "#viewNotesDrawer" )
        .addClass( "translate-x-full" );

    $( "#viewNotesOverlay" )
        .addClass( "hidden" );
}

$( document ).on( "click", "#closeViewNotesDrawer", function ()
{
    closeViewNotesDrawer();
} );

$( document ).on( "click", "#viewNotesOverlay", function ()
{
    closeViewNotesDrawer();
} );

function closeNotesDrawer ()
{
    $( "#notesDrawerBackdrop" )
        .removeClass( "opacity-100" )
        .addClass( "opacity-0" );

    $( "#notesDrawerPanel" )
        .removeClass( "translate-x-0" )
        .addClass( "translate-x-full" );

    setTimeout( function ()
    {
        $( "#notesDrawer" ).addClass( "invisible" );
    }, 300 );
}

$( "#closeNotesDrawer" ).on( "click", function ()
{
    closeNotesDrawer();
} );

$( "#cancelNotes" ).on( "click", function ()
{
    closeNotesDrawer();
} );

$( "#notesDrawerBackdrop" ).on( "click", function ()
{
    closeNotesDrawer();
} );


function updateNotesCharacterCount ()
{
    const notes = $( "#approvalNotes" ).val() || "";
    const length = notes.length;

    $( "#notesCharacterCount" ).text( `${ length } / 2000` );
}


$( "#approvalNotes" ).on( "input", function ()
{
    updateNotesCharacterCount();
} );

$( "#saveApprovalNotes" ).on( "click", function ()
{
    const trrsId = $( "#notesDrawer" ).data( "trrsId" );
    const approvalId = $( "#notesDrawer" ).data( "approvalId" );
    const notes = $( "#approvalNotes" ).val().trim();

    if ( !notes )
    {
        alert( "Please enter notes." );
        return;
    }

    $.ajax( {
        url: "/TRRF/UpdateApprovalNotes",
        type: "POST",
        data: {
            trrsId: trrsId,
            approvalId: approvalId,
            notes: notes
        },
        success: function ( response )
        {
            if ( response.success )
            {
                closeNotesDrawer();

                // Reload the grid
                mainTable.setData();
            }
            else
            {
                alert( response.message );
            }
        },
        error: function ()
        {
            alert( "An error occurred while saving the notes." );
        }
    } );
} );