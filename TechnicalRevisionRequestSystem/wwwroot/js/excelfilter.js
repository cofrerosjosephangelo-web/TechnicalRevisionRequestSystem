class ExcelFilter
{

    static activePopup = null;

    static filters = {};

    static dateFilter = {
        start: null,
        end: null
    };

    static applyFilters ( table )
    {

        table.clearFilter();

        const activeFilters = Object.entries( ExcelFilter.filters )
            .filter( ( [ _, values ] ) => values && values.length > 0 );

        const hasDateFilter =
            ExcelFilter.dateFilter &&
            (
                ExcelFilter.dateFilter.start ||
                ExcelFilter.dateFilter.end
            );

        if ( activeFilters.length === 0 && !hasDateFilter )
        {
            return;
        }

        table.setFilter( function ( data )
        {

            // Normal Excel filters
            const columnPass = activeFilters.every( ( [ field, values ] ) =>
            {
                return values.includes( String( data[ field ] ) );
            } );

            if ( !columnPass )
                return false;

            // Date filter
            const date = ExcelFilter.dateFilter;

            if ( date && ( date.start || date.end ) )
            {

                const rowDate = String( data.dateprepared ).substring( 0, 10 );

                if ( date.start && rowDate < date.start )
                    return false;

                if ( date.end && rowDate > date.end )
                    return false;
            }

            return true;

        } );

    }

    static close ()
    {

        if ( ExcelFilter.activePopup )
        {


            ExcelFilter.activePopup.remove();

            ExcelFilter.activePopup = null;
            ExcelFilter.activeColumn = null;

        }

        document.removeEventListener(
            "click",
            ExcelFilter.documentClick
        );

    }

    static open ( column, table )
    {

        ExcelFilter.close();

        const popup = document.createElement( "div" );
        popup.className = "excel-filter-popup";

        const field = column.getField();

        const isDateColumn = field === "dateprepared";

        const selectedValues =
            ExcelFilter.filters[ field ] || [];

        const currentFilter = ExcelFilter.filters[ field ];

        // Temporarily remove it
        delete ExcelFilter.filters[ field ];

        // Apply remaining filters
        ExcelFilter.applyFilters( table );

        // Get available values
        const values = [ ...new Set(
            table.getRows( "active" )
                .map( row => row.getData()[ field ] )
                .filter( v => v != null && v !== "" )
                .map( v =>
                {

                    if ( isDateColumn )
                    {

                        const date = new Date( v );

                        const mm = String( date.getMonth() + 1 ).padStart( 2, "0" );
                        const dd = String( date.getDate() ).padStart( 2, "0" );
                        const yyyy = date.getFullYear();

                        return `${ mm }-${ dd }-${ yyyy }`;
                    }

                    return String( v );

                } )
        ) ];

        // Restore filter
        if ( currentFilter )
        {
            ExcelFilter.filters[ field ] = currentFilter;
        }

        // Restore the original filtered view
        ExcelFilter.applyFilters( table );

        const isNumeric = table.getData().every( row =>
        {

            const value = row[ field ];

            return value == null ||
                value === "" ||
                !isNaN( Number( value ) );

        } );

        const ascIcon = isDateColumn
            ? "fa-arrow-down-1-9"
            : isNumeric
                ? "fa-arrow-down-1-9"
                : "fa-arrow-down-a-z";

        const descIcon = isDateColumn
            ? "fa-arrow-down-9-1"
            : isNumeric
                ? "fa-arrow-down-9-1"
                : "fa-arrow-down-z-a";

        const ascText = isDateColumn
            ? "Sort Oldest to Newest"
            : isNumeric
                ? "Sort Smallest to Largest"
                : "Sort A to Z";

        const descText = isDateColumn
            ? "Sort Newest to Oldest"
            : isNumeric
                ? "Sort Largest to Smallest"
                : "Sort Z to A";

        const dateSection = isDateColumn
            ? `
        <div class="excel-date-section">

            <div class="excel-date-title">
                <i class="fa-solid fa-calendar-days me-1"></i>
                Date Range
            </div>

            <input
                type="date"
                class="excel-date-from"
                value="${ ExcelFilter.dateFilter?.start || "" }"
            >

            <input
                type="date"
                class="excel-date-to"
                value="${ ExcelFilter.dateFilter?.end || "" }"
            >

            <hr class="excel-divider">

        </div>
    `
            : "";

        popup.innerHTML = `
<div class="excel-filter-sort">

    <div class="excel-menu-item excel-sort-asc">
        <span class="excel-menu-icon">
            <i class="fa-solid ${ ascIcon } fa-fw"></i>
        </span>

        <span class="excel-menu-text">
            ${ ascText }
        </span>
    </div>

    <div class="excel-menu-item excel-sort-desc">
        <span class="excel-menu-icon">
            <i class="fa-solid ${ descIcon } fa-fw"></i>
        </span>

        <span class="excel-menu-text">
            ${ descText }
        </span>
    </div>

</div>

${dateSection}

<div class="excel-filter-header">

    <input
        type="text"
        class="excel-filter-search"
        placeholder="🔍 Search..."
    >

</div>

<div class="excel-filter-body"></div>

<div class="excel-filter-footer">

    <button class="excel-clear">
        Clear
    </button>

    <button class="excel-apply">
        Apply
    </button>

</div>
`;

        document.body.appendChild( popup );

        ExcelFilter.activePopup = popup;
        ExcelFilter.activeColumn = field;

        const rect = column.getElement().getBoundingClientRect();

        const popupWidth = 340; // or popup.offsetWidth after appending

        let left = rect.left;
        let top = rect.bottom + 5;

        // Prevent overflow on the right
        if ( left + popupWidth > window.innerWidth )
        {

            left = window.innerWidth - popupWidth - 10;

        }

        // Prevent overflow on the left
        if ( left < 10 )
        {

            left = 10;

        }

        popup.style.left = `${ left }px`;
        popup.style.top = `${ top }px`;

        //------------------------------------------------

        const body = popup.querySelector( ".excel-filter-body" );

      

        function draw ( filter = "" )
        {

            body.innerHTML = `
<label class="excel-item excel-select-all">

    <input
        type="checkbox"
        class="excel-select-all-checkbox"
    >

    <strong>Select All</strong>

</label>

<hr class="excel-divider">
`;

            values
                .filter( v => String( v ).toLowerCase().includes( filter.toLowerCase() ) )
                .forEach( v =>
                {

                    body.insertAdjacentHTML(
                        "beforeend",
                        `
                        <label class="excel-item">

                            <input
    type="checkbox"
    value="${v }"
    ${ selectedValues.includes( String( v ) ) ? "checked" : ""}
>

                            ${ v }

                        </label>
                        `
                    );

                } );

            const selectAll = body.querySelector( ".excel-select-all-checkbox" );

            const checkboxes = body.querySelectorAll(
                "input[type=checkbox]:not(.excel-select-all-checkbox)"
            );

            // Initial state
            selectAll.checked =
                checkboxes.length > 0 &&
                [ ...checkboxes ].every( c => c.checked );

            // Select / Unselect all
            selectAll.addEventListener( "change", function ()
            {

                checkboxes.forEach( cb =>
                {

                    cb.checked = this.checked;

                } );

            } );

            // Keep Select All updated
            checkboxes.forEach( cb =>
            {

                cb.addEventListener( "change", () =>
                {

                    selectAll.checked =
                        [ ...checkboxes ].every( c => c.checked );

                } );

            } );

        }

        draw();

        popup
            .querySelector( ".excel-filter-search" )
            .addEventListener( "keyup", e =>
            {

                draw( e.target.value );

            } );

        popup
            .querySelector( ".excel-clear" )
            .onclick = () =>
            {

                delete ExcelFilter.filters[ field ];

                if ( isDateColumn )
                {
                    ExcelFilter.dateFilter = {
                        start: null,
                        end: null
                    };
                }

                ExcelFilter.applyFilters( table );

                ExcelFilter.close();

            };

        popup
            .querySelector( ".excel-apply" )
            .onclick = () =>
            {

                const values = [];

                popup
                    .querySelectorAll( "input[type=checkbox]:checked" )
                    .forEach( x => values.push( x.value ) );

                if ( values.length === 0 )
                {
                    delete ExcelFilter.filters[ field ];
                }
                else
                {
                    ExcelFilter.filters[ field ] = values;
                }

                if ( isDateColumn )
                {

                    ExcelFilter.dateFilter = {

                        start: popup.querySelector( ".excel-date-from" ).value,

                        end: popup.querySelector( ".excel-date-to" ).value

                    };

                }

                ExcelFilter.applyFilters( table );

                ExcelFilter.close();

            };

        popup.querySelector( ".excel-sort-asc" ).onclick = () =>
        {

            table.setSort( field, "asc" );

            ExcelFilter.close();

        };

        popup.querySelector( ".excel-sort-desc" ).onclick = () =>
        {

            table.setSort( field, "desc" );

            ExcelFilter.close();

        };

        document.removeEventListener(
            "click",
            ExcelFilter.documentClick
        );

        setTimeout( () =>
        {

            document.addEventListener(
                "click",
                ExcelFilter.documentClick
            );

        }, 10 );

    }

    static documentClick ( e )
    {

        if ( !ExcelFilter.activePopup )
            return;

        if ( ExcelFilter.activePopup.contains( e.target ) )
            return;

        ExcelFilter.close();

        document.removeEventListener(
            "click",
            ExcelFilter.documentClick
        );

    }

}