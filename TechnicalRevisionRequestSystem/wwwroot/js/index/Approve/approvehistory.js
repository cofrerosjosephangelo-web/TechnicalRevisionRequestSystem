function loadApprovalHistory ( trrsId, approverPart )
{
    $.ajax( {
        url: "/TRRF/GetApprovalHistory",
        type: "GET",
        data: {
            trrsId: trrsId,
            approverPart: approverPart
        },

        success: function ( data )
        {
            let html = "";
            let navHtml = "";

            if ( !data || data.length === 0 )
            {
                $( "#approvalHistoryContainer" ).html( "" );
                $( "#approvalQuickNavItems" ).html( "" );

                return;
            }

            data.forEach( item =>
            {
                // ============================================
                // Quick Navigation Item
                // ============================================

                navHtml += `
                    <button
                        type="button"
                        class="approval-nav-item
                               w-full
                               text-left
                               px-3 py-2
                               rounded-xl
                               text-xs
                               text-slate-600
                               hover:bg-white/50
                               transition"
                        data-target="approval-${ item.approver_part }">

                        <i class="fa-solid fa-chevron-right
                                  text-[10px]
                                  me-1"></i>

                        ${ item.approver_role ?? "Approval" }

                    </button>
                `;

                // ============================================
                // Approval Card
                // ============================================

                html += `
                    <div id="approval-${ item.approver_part }"
                         class="glass-card p-5 approval-section">

                        <!-- Header -->
                        <div class="flex items-center justify-between
                                    pb-3 mb-4
                                    border-b border-white/40">

                            <div class="flex items-center gap-2">

                                <i class="fa-solid fa-user-check text-slate-600"></i>

                                <h3 class="text-sm font-bold uppercase tracking-wide text-slate-700">
                                    ${ item.approver_role ?? "Approval" }
                                </h3>

                            </div>

                            ${ item.has_attachment
                        ?
                        `
                                <button
                                    type="button"
                                    class="btnViewApproverAttachment
                                           px-3 py-1.5
                                           text-xs
                                           font-medium
                                           rounded-lg
                                           bg-blue-500/10
                                           text-blue-700
                                           border border-blue-300/50
                                           hover:bg-blue-500/20
                                           transition"
                                    data-trrs-id="${ item.trrs_id }"
                                    data-part="${ item.approver_part }">

                                    <i class="fa fa-paperclip me-1"></i>
                                    View Attachment(s)

                                </button>
                                `
                        :
                        ""
                    }

                        </div>

                        <!-- Approval Info -->
                        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">

                            <div>

                                <label class="block text-xs text-slate-500 uppercase mb-1">
                                    Approved By
                                </label>

                                <input type="text"
                                       readonly
                                       value="${ item.approved_by ?? "" }"
                                       class="w-full px-3 py-2 rounded-lg border border-dark/40 bg-white/40 text-sm text-slate-700" />

                            </div>

                            <div>

                                <label class="block text-xs text-slate-500 uppercase mb-1">
                                    Approved Date
                                </label>

                                <input type="text"
                                       readonly
                                       value="${ formatDate( item.approved_date ) }"
                                       class="w-full px-3 py-2 rounded-lg border border-dark/40 bg-white/40 text-sm text-slate-700" />

                            </div>

                        </div>

                        <!-- Remarks -->
                        <div class="mt-4">

                            <label class="block text-xs text-slate-500 uppercase mb-2">
                                Remarks
                            </label>

                            <textarea readonly
                                      rows="5"
                                      class="w-full rounded-lg border border-dark/40 bg-white/30 p-3 resize-none">${ item.approver_remarks ?? "" }</textarea>

                        </div>

                    </div>
                `;
            } );

            // ============================================
            // Render Approval History
            // ============================================

            $( "#approvalHistoryContainer" ).html( html );

            // ============================================
            // Render Quick Navigation
            // ============================================

            $( "#approvalQuickNavItems" ).html( navHtml );
        },

        error: function ( xhr )
        {
            console.error( xhr );

            $( "#approvalHistoryContainer" ).html( "" );
            $( "#approvalQuickNavItems" ).html( "" );
        }
    } );
}

function loadRevisionHistory ( trrsId, deviationId )
{
    $.ajax( {
        url: "/TRRF/GetRevisionHistory",
        type: "GET",
        data: {
            trrsId: trrsId,
            deviationId: deviationId
        },

        success: function ( data )
        {
            let html = "";

            if ( !data || data.length === 0 )
            {
                $( "#revisionHistoryContainer" ).html( `
                    <div class="glass-card p-6 text-center text-slate-500">
                        No revision history found.
                    </div>
                `);

                return;
            }

            data.forEach( item =>
            {
                html += `
                    <div class="glass-card p-5">

                        <!-- Header -->
                        <div class="flex items-center justify-between pb-3 mb-4 border-b border-white/40">

                            <div class="flex items-center gap-2">

                                <i class="fa-solid fa-code-branch text-indigo-600"></i>

                                <h3 class="text-sm font-bold uppercase tracking-wide text-slate-700">
                                    Revision ${ item.deviation_version }
                                </h3>

                            </div>

                            <span class="text-xs text-slate-500">
                                ${ formatDate( item.deviation_date ) }
                            </span>

                        </div>

                        <!-- Details -->
                        <div>

                            <label class="block text-xs text-slate-500 uppercase mb-2">
                                Revision Details
                            </label>

                            <textarea
                                readonly
                                rows="6"
                                class="w-full rounded-lg border border-dark/40 bg-white/30 p-3 resize-none">${ item.deviation_details ?? "" }</textarea>

                        </div>

                    </div>
                `;
            } );

            $( "#revisionHistoryContainer" ).html( html );
        },

        error: function ( xhr )
        {
            console.error( xhr );

            $( "#revisionHistoryContainer" ).html( `
                <div class="glass-card p-6 text-center text-red-500">
                    Failed to load revision history.
                </div>
            `);
        }
    } );
}


$( document ).on(
    "click",
    ".approval-nav-item",
    function ()
    {
        const targetId = $( this ).data( "target" );

        const container = document.getElementById( "leftDrawerBody" );
        const target = document.getElementById( targetId );

        if ( !container || !target )
            return;

        const scrollPosition =
            target.offsetTop -
            container.offsetTop -
            20;

        container.scrollTo( {
            top: scrollPosition,
            behavior: "smooth"
        } );
    }
);

const body = document.getElementById( "leftDrawerBody" );
const nav = document.getElementById( "approvalQuickNav" );

//body.addEventListener( "scroll", () =>
//{
//    nav.style.transform =
//        `translateY(${ body.scrollTop - 350 }px)`;
//} );



$( document ).on( "change", "#isApproved", function ()
{
    if ( $( this ).is( ":checked" ) )
    {
        $( "#isNotApproved" ).prop( "checked", false );
    }
} );

$( document ).on( "change", "#isNotApproved", function ()
{
    if ( $( this ).is( ":checked" ) )
    {
        $( "#isApproved" ).prop( "checked", false );
    }
} );