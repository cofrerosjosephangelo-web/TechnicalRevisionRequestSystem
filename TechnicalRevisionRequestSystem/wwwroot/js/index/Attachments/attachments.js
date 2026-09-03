$( document ).on( "click", "#btnViewRootCauseAttachment", function ()
{
    loadAttachments( currentTrrsId, "rootcause" );
} );

$( document ).on( "click", "#btnViewActionPlanAttachment", function ()
{
    loadAttachments( currentTrrsId, "actionplan" );
} );

$( document ).on( "click", "#btnViewMomAttachment", function ()
{
    loadAttachments( currentTrrsId, "mom" );
} );

$( document ).on( "click", ".btnViewApproverAttachment", function ()
{
    const trrsId = $( this ).data( "trrs-id" );
    const part = $( this ).data( "part" );

    loadApproverAttachments(
        trrsId,
        part
    );
} );

$( document ).on(
    "click",
    "#btnCloseAttachmentSheet, #attachmentSheetOverlay",
    function ()
    {
        $( "#attachmentSheet" ).addClass( "hidden" );
        $( "#attachmentSheetOverlay" ).addClass( "hidden" );
    }
);

$( document ).on( "click", ".downloadAttachmentBtn", function ()
{
    const attachmentId = $( this ).data( "id" );
    const type = $( this ).data( "type" );

    window.location.href =
        `/TRRF/DownloadAttachment?attachmentId=${ attachmentId }&type=${ type }`;
} );

$( document ).on( "click", ".previewAttachmentBtn", function ()
{
    const attachmentId = $( this ).data( "id" );
    const type = $( this ).data( "type" );

    window.open(
        `/TRRF/PreviewAttachment?attachmentId=${ attachmentId }&type=${ type }`,
        "_blank"
    );
} );

function loadAttachments ( trrsId, type )
{
    showAttachmentLoader();

    $.ajax( {
        url: "/TRRF/GetAttachments",
        type: "GET",

        data:
        {
            trrsId: trrsId,
            type: type
        },

        success: function ( response )
        {
            if ( !response.success )
            {
                Swal.fire(
                    "Error",
                    response.message,
                    "error"
                );

                return;
            }

            renderAttachmentList(
                response.data,
                type
            );
        },

        error: function ()
        {
            Swal.fire(
                "Error",
                "Unable to load attachments.",
                "error"
            );
        }
    } );
}

function loadApproverAttachments ( trrsId, approvingPart )
{
    showAttachmentLoader();

    $.ajax( {
        url: "/TRRF/GetApproverAttachments",
        type: "GET",

        data:
        {
            trrsId: trrsId,
            approvingPart: approvingPart
        },

        success: function ( response )
        {
            if ( !response.success )
            {
                Swal.fire(
                    "Error",
                    response.message,
                    "error"
                );

                return;
            }

            renderAttachmentList(
                response.data,
                "approver"
            );
        },

        error: function ()
        {
            Swal.fire(
                "Error",
                "Unable to load attachments.",
                "error"
            );
        }
    } );
}

function showAttachmentLoader ()
{
    $( "#attachmentList" ).html( `
        <div class="text-center py-4 text-slate-500">
            Loading attachments...
        </div>
    `);

    $( "#attachmentSheetOverlay" ).removeClass( "hidden" );
    $( "#attachmentSheet" ).removeClass( "hidden" );
}

function renderAttachmentList ( files, type )
{
    let html = "";

    if ( !files || files.length === 0 )
    {
        $( "#attachmentList" ).html( `
            <div class="text-center py-6 text-slate-500">
                No attachments found.
            </div>
        `);

        return;
    }

    files.forEach( file =>
    {
        const previewAllowed =
            canPreview( file.file_name );

        html += `
            <div class="flex items-center justify-between
                        p-3 rounded-xl
                        border border-slate-200/50
                        bg-white/40">

                <div class="flex items-center gap-2">

                    <i class="fa fa-paperclip text-slate-500"></i>

                    <span class="text-sm text-slate-700">
                        ${ file.file_name }
                    </span>

                </div>

                <div class="flex gap-2">

                    ${ previewAllowed ? `
                        <button
                            class="previewAttachmentBtn
                                   px-3 py-1 rounded-lg
                                   border border-slate-300
                                   hover:bg-slate-100"
                            data-id="${ file.attachment_id }"
                            data-type="${ type }">
                            <i class="fa fa-eye"></i>
                        </button>
                    ` : "" }

                    <button
                        class="downloadAttachmentBtn
                               px-3 py-1 rounded-lg
                               bg-blue-600 text-white
                               hover:bg-blue-700"
                        data-id="${ file.attachment_id }"
                        data-type="${ type }">
                        <i class="fa fa-download"></i>
                    </button>

                </div>

            </div>
        `;
    } );

    $( "#attachmentList" ).html( html );
}

function canPreview ( fileName )
{
    if ( !fileName )
    {
        return false;
    }

    const ext =
        fileName.split( "." ).pop().toLowerCase();

    return [
        "pdf",
        "jpg",
        "jpeg",
        "png",
        "gif",
        "webp"
    ].includes( ext );
}