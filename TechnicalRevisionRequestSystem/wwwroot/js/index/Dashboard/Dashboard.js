function animateValue ( id, start, end, duration = 800, decimals = 0 )
{

    const obj = document.getElementById( id );

    if ( !obj ) return;

    let startTime = null;

    function animation ( currentTime )
    {

        if ( !startTime )
            startTime = currentTime;

        const progress = Math.min( ( currentTime - startTime ) / duration, 1 );

        const value = start + ( end - start ) * progress;

        obj.innerText = value.toLocaleString( undefined, {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        } );

        if ( progress < 1 )
            requestAnimationFrame( animation );
    }

    requestAnimationFrame( animation );

}

$( document ).ready( function ()
{

    $( "#dashboardYear, #dashboardMonth" ).on( "change", function ()
    {
        refreshDashboard();
    } );

} );





function loadDashboardSummary ()
{

    $.ajax( {

        url: '/TRRF/GetDashboardSummary',

        type: 'GET',

        data: {

            year: $( '#dashboardYear' ).val(),
            month: $( '#dashboardMonth' ).val()

        },

        success: function ( r )
        {

            if ( !r.success )
                return;

            animateValue( "kpiTotalRequests", 0, r.totalRequests );

            animateValue( "kpiPD", 0, r.pdRequests );

            animateValue( "kpiMP", 0, r.mpRequests );

            animateValue( "kpiCustomers", 0, r.totalCustomers );

            animateValue( "kpiProducts", 0, r.totalProducts );

            animateValue( "kpiMolds", 0, r.totalMolds );

            animateValue( "kpiRevision", 0, r.averageRevision, 900, 2 );

        }

    } );

}

let monthlyChart = null;

function loadMonthlyChart ()
{
    $.ajax( {

        url: "/TRRF/GetMonthlyRequests",

        type: "GET",

        data: {

            year: $( "#dashboardYear" ).val()

        },

        success: function ( result )
        {
            const months = [];
            const totals = [];

            result.forEach( x =>
            {
                months.push( x.monthName );
                totals.push( x.totalRequests );
            } );

            renderMonthlyChart( months, totals );
        }

    } );
}


function renderMonthlyChart ( months, totals )
{
    const chartDom = document.getElementById( "monthlyChart" );

    if ( !monthlyChart )
        monthlyChart = echarts.init( chartDom );

    const option =
    {
        animation: true,

        title:
        {
            text: "Monthly Requests",
            left: "center",
            textStyle:
            {
                fontSize: 18,
                fontWeight: "bold",
                color: "#334155"
            }
        },

        tooltip:
        {
            trigger: "axis"
        },

        grid:
        {
            left: 50,
            right: 25,
            top: 70,
            bottom: 45
        },

        xAxis:
        {
            type: "category",

            boundaryGap: false,

            data: months,

            axisLine:
            {
                lineStyle:
                {
                    color: "#CBD5E1"
                }
            }
        },

        yAxis:
        {
            type: "value",

            splitLine:
            {
                lineStyle:
                {
                    color: "#E2E8F0"
                }
            }
        },

        series:
            [
                {
                    name: "Requests",

                    type: "line",

                    smooth: true,

                    symbol: "circle",

                    symbolSize: 10,

                    data: totals,

                    lineStyle:
                    {
                        width: 4,
                        color: "#0EA5E9"
                    },

                    itemStyle:
                    {
                        color: "#0284C7"
                    },

                    areaStyle:
                    {
                        opacity: 0.15
                    }
                }
            ]
    };

    monthlyChart.setOption( option );
}

window.addEventListener( "resize", function ()
{
    monthlyChart?.resize();
    statusChart?.resize();
    customerChart?.resize();
    productChart?.resize();
    revisionChart?.resize();
    moldChart?.resize();
    deviationChart?.resize();
    topRequestedProductsTable?.redraw();
    monthlyTrendTable?.redraw();
    detailedBreakdownTable?.redraw();
	moldLifeUtilizationChart?.resize();

} );



let statusChart = null;

function loadStatusChart ()
{
    $.ajax( {

        url: "/TRRF/GetRequestsByType",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            renderStatusChart( result );
        }

    } );
}


function renderStatusChart ( data )
{
    const chartDom = document.getElementById( "statusChart" );

    if ( !statusChart )
        statusChart = echarts.init( chartDom );

    const option =
    {
        title:
        {
            text: "Request Type",
            left: "center",
            top: 10,
            textStyle:
            {
                color: "#334155",
                fontSize: 18,
                fontWeight: "bold"
            }
        },

        tooltip:
        {
            trigger: "item",
            formatter: "{b}<br/>Requests : {c}<br/>({d}%)"
        },

        legend:
        {
            orient: "vertical",
            left: 10,
            top: "middle"
        },

        series:
            [
                {
                    name: "Requests",

                    type: "pie",

                    radius: [ "55%", "75%" ],

                    center: [ "62%", "55%" ],

                    avoidLabelOverlap: true,

                    itemStyle:
                    {
                        borderRadius: 8,
                        borderColor: "#fff",
                        borderWidth: 2
                    },

                    label:
                    {
                        show: true,
                        formatter: "{b}\n{c}"
                    },

                    emphasis:
                    {
                        label:
                        {
                            fontSize: 18,
                            fontWeight: "bold"
                        }
                    },

                    data: data
                }
            ]
    };

    statusChart.setOption( option );
}


let customerChart = null;

function loadCustomerChart ()
{
    $.ajax( {

        url: "/TRRF/GetRequestsByCustomer",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            const customers = [];
            const totals = [];

            result.forEach( x =>
            {
                customers.push( x.customerName );
                totals.push( x.totalRequests );
            } );

            renderCustomerChart( customers, totals );
        }

    } );
}

function renderCustomerChart ( customers, totals )
{
    const chartDom = document.getElementById( "customerChart" );

    if ( !customerChart )
        customerChart = echarts.init( chartDom );

    const option =
    {
        title:
        {
            text: "Requests by Customer",
            left: "center",
            textStyle:
            {
                color: "#334155",
                fontSize: 18,
                fontWeight: "bold"
            }
        },

        tooltip:
        {
            trigger: "axis",
            axisPointer:
            {
                type: "shadow"
            }
        },

        grid:
        {
            left: 160,
            right: 30,
            top: 70,
            bottom: 35
        },

        xAxis:
        {
            type: "value",

            splitLine:
            {
                lineStyle:
                {
                    color: "#E2E8F0"
                }
            }
        },

        yAxis:
        {
            type: "category",

            data: customers,

            axisTick:
            {
                show: false
            },

            axisLine:
            {
                show: false
            }
        },

        series:
            [
                {
                    type: "bar",

                    data: totals,

                    barWidth: 20,

                    showBackground: true,

                    backgroundStyle:
                    {
                        color: "#F1F5F9"
                    },

                    itemStyle:
                    {
                        borderRadius: [ 0, 8, 8, 0 ],

                        color: "#3B82F6"
                    },

                    label:
                    {
                        show: true,
                        position: "right",
                        color: "#334155",
                        fontWeight: "bold"
                    }
                }
            ]
    };

    customerChart.setOption( option );
}


let productChart = null;


function loadProductChart ()
{
    $.ajax( {

        url: "/TRRF/GetRequestsByProduct",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            const products = [];
            const totals = [];

            result.forEach( x =>
            {
                products.push( x.productName );
                totals.push( x.totalRequests );
            } );

            renderProductChart( products, totals );
        }

    } );
}

function renderProductChart ( products, totals )
{
    const chartDom = document.getElementById( "productChart" );

    if ( !chartDom )
        return;

    if ( !productChart )
        productChart = echarts.init( chartDom );

    const option =
    {
        title:
        {
            text: "Requests by Product",
            left: "center",
            textStyle:
            {
                color: "#334155",
                fontSize: 18,
                fontWeight: "bold"
            }
        },

        tooltip:
        {
            trigger: "axis",
            axisPointer:
            {
                type: "shadow"
            }
        },

        grid:
        {
            left: 170,
            right: 30,
            top: 70,
            bottom: 35
        },

        xAxis:
        {
            type: "value",

            splitLine:
            {
                lineStyle:
                {
                    color: "#E2E8F0"
                }
            }
        },

        yAxis:
        {
            type: "category",

            data: products,

            axisTick:
            {
                show: false
            },

            axisLine:
            {
                show: false
            }
        },

        series:
            [
                {
                    type: "bar",

                    data: totals,

                    barWidth: 18,

                    showBackground: true,

                    backgroundStyle:
                    {
                        color: "#F8FAFC"
                    },

                    itemStyle:
                    {
                        color: "#F59E0B",

                        borderRadius: [ 0, 8, 8, 0 ]
                    },

                    label:
                    {
                        show: true,
                        position: "right",
                        color: "#334155",
                        fontWeight: "bold"
                    }
                }
            ]
    };

    productChart.setOption( option );
}


let revisionChart = null;

function loadRevisionChart ()
{
    $.ajax( {

        url: "/TRRF/GetRevisionDistribution",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            const revisions = [];
            const totals = [];

            result.forEach( x =>
            {
                revisions.push( "Rev " + x.revision );
                totals.push( x.totalRequests );
            } );

            renderRevisionChart( revisions, totals );
        }

    } );
}

function renderRevisionChart ( revisions, totals )
{
    const chartDom = document.getElementById( "revisionChart" );

    if ( !chartDom )
        return;

    if ( !revisionChart )
        revisionChart = echarts.init( chartDom );

    const option =
    {
        animation: true,

        title:
        {
            text: "Revision Distribution",
            left: "center",
            textStyle:
            {
                color: "#334155",
                fontSize: 18,
                fontWeight: "bold"
            }
        },

        tooltip:
        {
            trigger: "axis",
            axisPointer:
            {
                type: "shadow"
            }
        },

        grid:
        {
            left: 60,
            right: 25,
            top: 70,
            bottom: 45
        },

        xAxis:
        {
            type: "category",

            data: revisions,

            axisTick:
            {
                alignWithLabel: true
            }
        },

        yAxis:
        {
            type: "value",

            splitLine:
            {
                lineStyle:
                {
                    color: "#E2E8F0"
                }
            }
        },

        series:
            [
                {
                    type: "bar",

                    data: totals,

                    barWidth: "45%",

                    itemStyle:
                    {
                        color: "#EF4444",

                        borderRadius: [ 8, 8, 0, 0 ]
                    },

                    label:
                    {
                        show: true,
                        position: "top",
                        fontWeight: "bold",
                        color: "#334155"
                    }
                }
            ]
    };

    revisionChart.setOption( option );
}


let moldChart = null;

function loadMoldChart ()
{
    $.ajax( {

        url: "/TRRF/GetRequestsByMold",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            const molds = [];
            const totals = [];

            result.forEach( x =>
            {
                molds.push( x.moldNo );
                totals.push( x.totalRequests );
            } );

            renderMoldChart( molds, totals );
        }

    } );
}

function renderMoldChart ( molds, totals )
{
    const chartDom = document.getElementById( "moldChart" );

    if ( !chartDom )
        return;

    if ( !moldChart )
        moldChart = echarts.init( chartDom );

    const option =
    {
        animation: true,

        title:
        {
            text: "Requests by Mold",
            left: "center",
            textStyle:
            {
                color: "#334155",
                fontSize: 18,
                fontWeight: "bold"
            }
        },

        tooltip:
        {
            trigger: "axis",
            axisPointer:
            {
                type: "shadow"
            }
        },

        grid:
        {
            left: 170,
            right: 30,
            top: 70,
            bottom: 35
        },

        xAxis:
        {
            type: "value",

            splitLine:
            {
                lineStyle:
                {
                    color: "#E2E8F0"
                }
            }
        },

        yAxis:
        {
            type: "category",

            data: molds,

            axisTick:
            {
                show: false
            },

            axisLine:
            {
                show: false
            }
        },

        series:
            [
                {
                    type: "bar",

                    data: totals,

                    barWidth: 18,

                    showBackground: true,

                    backgroundStyle:
                    {
                        color: "#F8FAFC"
                    },

                    itemStyle:
                    {
                        color: "#8B5CF6",
                        borderRadius: [ 0, 8, 8, 0 ]
                    },

                    label:
                    {
                        show: true,
                        position: "right",
                        color: "#334155",
                        fontWeight: "bold"
                    }
                }
            ]
    };

    moldChart.setOption( option );
}

let deviationChart = null;

function loadDeviationChart ()
{
    $.ajax( {

        url: "/TRRF/GetDeviationAnalytics",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            renderDeviationChart( result );
        }

    } );
}

function renderDeviationChart ( data )
{
    const chartDom = document.getElementById( "deviationChart" );

    if ( !chartDom )
        return;

    if ( !deviationChart )
        deviationChart = echarts.init( chartDom );

    const total = data.reduce( ( sum, x ) => sum + x.value, 0 );

    const option =
    {
        title:
        {
            text: "Deviation/Revision Analytics",
            subtext: total + " Requests",
            left: "center",
            top: 10,
            textStyle:
            {
                fontSize: 18,
                fontWeight: "bold",
                color: "#334155"
            },
            subtextStyle:
            {
                color: "#64748B"
            }
        },

        tooltip:
        {
            trigger: "item",
            formatter: "{b}<br/>Requests : {c}<br/>({d}%)"
        },

        legend:
        {
            orient: "horizontal",
            bottom: 10
        },

        color:
            [
                "#10B981",
                "#F59E0B"
            ],

        series:
            [
                {
                    name: "TRRS",

                    type: "pie",

                    radius:
                        [
                            "55%",
                            "78%"
                        ],

                    center:
                        [
                            "50%",
                            "50%"
                        ],

                    itemStyle:
                    {
                        borderColor: "#fff",
                        borderWidth: 3,
                        borderRadius: 10
                    },

                    label:
                    {
                        formatter: "{b}\n{c}"
                    },

                    emphasis:
                    {
                        scale: true,

                        scaleSize: 8
                    },

                    data: data
                }
            ]
    };

    deviationChart.setOption( option );
}

let topRequestedProductsTable = null;

function loadTopRequestedProductsTable ()
{
    if ( topRequestedProductsTable )
    {
        topRequestedProductsTable.setData(
            "/TRRF/GetTopRequestedProducts",
            {
                year: $( "#dashboardYear" ).val(),
                month: $( "#dashboardMonth" ).val()
            } );

        return;
    }

    topRequestedProductsTable = new Tabulator( "#topRequestedProductsTable",
        {
            ajaxURL: "/TRRF/GetTopRequestedProducts",

            ajaxParams:
            {
                year: $( "#dashboardYear" ).val(),
                month: $( "#dashboardMonth" ).val()
            },

            ajaxResponse: function ( url, params, response )
            {
                return response.data;
            },

            layout: "fitColumns",

            height: 420,

            movableColumns: true,

            placeholder: "No records found.",

            columns:
                [
                    {
                        title: "#",
                        field: "no",
                        hozAlign: "center",
                        width: 70
                    },

                    {
                        title: "Customer",
                        field: "customer",
                        hozAlign: "center",
                        headerHozAlign: "center",
                        headerFilter: "input"
                    },

                    {
                        title: "Product Name",
                        field: "productName",
                        hozAlign: "center",
                        headerHozAlign: "center",
                        headerFilter: "input"
                    },

                    {
                        title: "Mold No.",
                        field: "moldNo",
                        headerFilter: "input",
                        hozAlign: "center",
                        headerHozAlign: "center",
                        width: 170
                    },

                    {
                        title: "TRRF Count",
                        field: "trrfCount",
                        hozAlign: "center",
                        headerHozAlign: "center",
                        sorter: "number",
                        width: 140
                    }
                ]
        } );
}

let monthlyTrendTable = null;
let detailedBreakdownTable = null;


function loadMonthlyTrendTable ()
{
    $.get( "/TRRF/GetMonthlyTrend",
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },
        function ( response )
        {
            const data = response.data;

            if ( !data.length )
                return;

            const columns = [];

            Object.keys( data[ 0 ] ).forEach( key =>
            {
                columns.push( {
                    title: key,
                    field: key,
                    hozAlign: "center",
                    headerHozAlign: "center"
                } );
            } );

            if ( monthlyTrendTable )
            {
                monthlyTrendTable.setColumns( columns );
                monthlyTrendTable.setData( data );
                return;
            }

            monthlyTrendTable = new Tabulator( "#monthlyTrendTable",
                {
                    data: data,
                    layout: "fitColumns",
                    height: 300,
                    movableColumns: true,
                    placeholder: "No records found.",
                    columns: columns
                } );

        } );
}

function loadDetailedBreakdownTable ()
{
    $.get( "/TRRF/GetDetailedBreakdown",
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },
        function ( response )
        {
            const data = response.data;

            if ( !data.length )
                return;

            const columns = [];

            Object.keys( data[ 0 ] ).forEach( key =>
            {
                columns.push( {
                    title: key,
                    field: key,
                    hozAlign: "center",
                    headerHozAlign: "center"
                } );
            } );

            if ( detailedBreakdownTable )
            {
                detailedBreakdownTable.setColumns( columns );
                detailedBreakdownTable.setData( data );
                return;
            }

            detailedBreakdownTable = new Tabulator( "#detailedBreakdownTable",
                {
                    data: data,
                    layout: "fitColumns",
                    height: 420,
                    movableColumns: true,
                    placeholder: "No records found.",
                    columns: columns
                } );

        } );
}


let moldLifeUtilizationChart = null;

function loadTRRFDistributionByLifeUtilization ()
{
    $.ajax( {

        url: "/TRRF/GetTRRFDistributionByLifeUtilization",

        type: "GET",

        data:
        {
            year: $( "#dashboardYear" ).val(),
            month: $( "#dashboardMonth" ).val()
        },

        success: function ( result )
        {
            const stages = [];
            const trrfCounts = [];
            const cumulativePercentages = [];

            result.data.forEach( x =>
            {
                stages.push( x.condition_stage );

                trrfCounts.push( {
                    value: Number( x.trrf_count ),
                    stageId: Number( x.stage_id ),
                    stageName: x.condition_stage
                } );

                cumulativePercentages.push( Number( x.cumulative_percentage ) );
            } );

            renderTRRFDistributionByLifeUtilization(
                stages,
                trrfCounts,
                cumulativePercentages
            );
        }

    } );
}


function renderTRRFDistributionByLifeUtilization (
    stages,
    trrfCounts,
    cumulativePercentages )
{
    const chartDom = document.getElementById( "moldLifeUtilizationChart" );

    if ( !moldLifeUtilizationChart )
        moldLifeUtilizationChart = echarts.init( chartDom );

    const option =
    {
        animation: true,

        tooltip:
        {
            trigger: "axis"
        },

        legend:
        {
            top: 10,
            data:
                [
                    "TRRF Count",
                    "Cumulative %"
                ]
        },

        grid:
        {
            left: 60,
            right: 70,
            top: 70,
            bottom: 70
        },

        xAxis:
        {
            type: "category",

            data: stages,

            axisLabel:
            {
                interval: 0,
                rotate: 20
            }
        },

        yAxis:
            [
                {
                    type: "value",

                    name: "TRRF",

                    minInterval: 1,

                    splitLine:
                    {
                        lineStyle:
                        {
                            color: "#E2E8F0"
                        }
                    }
                },

                {
                    type: "value",

                    name: "Cumulative %",

                    min: 0,

                    max: 100,

                    interval: 20,

                    axisLabel:
                    {
                        formatter: "{value}%"
                    }
                }
            ],

        series:
            [
                {
                    name: "TRRF Count",

                    type: "bar",

                    data: trrfCounts,

                    barWidth: "55%",

                    itemStyle:
                    {
                        borderRadius: [ 6, 6, 0, 0 ],

                        color: function ( params )
                        {
                            switch ( params.data.stageId )
                            {
                                case 1:
                                    return "#00A859"; // New / Commissioning

                                case 2:
                                    return "#8CC63F"; // Early Life

                                case 3:
                                    return "#C0CA33"; // Prime Life

                                case 4:
                                    return "#FFC107"; // Mature Life

                                case 5:
                                    return "#F37023"; // Late Life

                                case 6:
                                    return "#E53E3E"; // End Of Life

                                case 7:
                                    return "#8B0000"; // Beyond Tool Life

                                default:
                                    return "#94A3B8";
                            }
                        }
                    },

                    label:
                    {
                        show: true,
                        position: "top",
                        color: "#374151",
                        fontWeight: "bold",
                        fontSize: 12,

                        formatter: function ( params )
                        {
                            return params.data.value;
                        }
                    }
                },

                {
                    name: "Cumulative %",

                    type: "line",

                    yAxisIndex: 1,

                    smooth: true,

                    symbol: "circle",

                    symbolSize: 10,

                    data: cumulativePercentages,

                    lineStyle:
                    {
                        width: 3,
                        color: "#2563EB"
                    },

                    itemStyle:
                    {
                        color: "#2563EB"
                    },

                    label:
                    {
                        show: true,
                        position: "right",
                        color: "#2563EB",
                        fontWeight: "bold",
                        fontSize: 11,

                        formatter: function ( params )
                        {
                            return params.value.toFixed( 2 ) + "%";
                        }
                    },

                    markLine:
                    {
                        silent: true,

                        symbol: "none",

                        lineStyle:
                        {
                            color: "#DC2626",
                            type: "dashed",
                            width: 2
                        },

                        label:
                        {
                            show: true,
                            formatter: "80%",
                            color: "#DC2626",
                            fontWeight: "bold"
                        },

                        data:
                            [
                                {
                                    yAxis: 80
                                }
                            ]
                    }
                }
            ]
    };

    moldLifeUtilizationChart.setOption( option, true );

    moldLifeUtilizationChart.off( "click" );

    moldLifeUtilizationChart.on( "click", function ( params )
    {
        if ( params.seriesType !== "bar" )
            return;

        openLifeUtilizationStageDrawer();

        loadLifeUtilizationStageTable(
            params.data.stageId,
            params.data.stageName
        );
    } );

    window.addEventListener( "resize", function ()
    {
        moldLifeUtilizationChart.resize();
    } );
}



let modalLifeUtilStageTable = null;

function loadLifeUtilizationStageTable ( stageId, stageName )
{
    $( "#lifeUtilizationStageTitle" ).text( stageName );

    if ( modalLifeUtilStageTable )
    {
        modalLifeUtilStageTable.setData(
            "/TRRF/GetTRRFDetailsByStageId",
            {
                stageId: stageId,
                year: $( "#dashboardYear" ).val(),
                month: $( "#dashboardMonth" ).val()
            } );

        return;
    }

    modalLifeUtilStageTable = new Tabulator( "#modalLifeUtilStageTable",
        {
            ajaxURL: "/TRRF/GetTRRFDetailsByStageId",

            ajaxParams:
            {
                stageId: stageId,
                year: $( "#dashboardYear" ).val(),
                month: $( "#dashboardMonth" ).val()
            },

            ajaxResponse: function ( url, params, response )
            {
                return response.data;
            },

            layout: "fitDataStretch",

            height: 650,

            movableColumns: true,

            placeholder: "No records found.",

            columns:
                [
                    {
                        title: "#",
                        formatter: "rownum",
                        width: 60,
                        hozAlign: "center",
                        frozen: true
                    },

                    {
                        title: "TRRF No",
                        field: "trrf_no",
                        headerFilter: "input",
                        minWidth: 140,
                        frozen: true,

                        formatter: function ( cell )
                        {
                            const value = cell.getValue();

                            if ( value == null || value === "" || typeof value === "object" )
                                return "N/A";

                            return value;
                        }
                    },

                    {
                        title: "Customer",
                        field: "customer_name",
                        headerFilter: "input",
                        minWidth: 220,
                        frozen: true
                    },

                    {
                        title: "Product",
                        field: "product_name",
                        headerFilter: "input",
                        minWidth: 220,
                        frozen: true
                    },

                    {
                        title: "Part Model",
                        field: "part_model_no",
                        headerFilter: "input",
                        minWidth: 180,
                        frozen: true
                    },

                    {
                        title: "Mold No.",
                        field: "mold_no",
                        hozAlign: "center",
                        minWidth: 140,
                        frozen: true
                    },

                    {
                        title: "Mold Tool Life",
                        field: "mold_tool_life",
                        hozAlign: "center",
                        minWidth: 160,
                        formatter: "money",
                        formatterParams:
                        {
                            thousand: ",",
                            precision: 0,
                            symbol: ""
                        }
                    },

                    {
                        title: "Utilization %",
                        field: "utilization_percentage",
                        hozAlign: "center",
                        minWidth: 150,

                        formatter: function ( cell )
                        {
                            const value = Number( cell.getValue() );

                            return isNaN( value )
                                ? "N/A"
                                : value.toFixed( 2 ) + "%";
                        }
                    },

                    {
                        title: "Guaranteed Life",
                        field: "guaranteed_life",
                        hozAlign: "center",
                        minWidth: 170,

                        formatter: "money",
                        formatterParams:
                        {
                            thousand: ",",
                            precision: 0,
                            symbol: ""
                        }
                    },

                    {
                        title: "Type",
                        field: "trrs_type",
                        hozAlign: "center",
                        minWidth: 90
                    },

                    {
                        title: "Prepared",
                        field: "date_prepared",
                        hozAlign: "center",
                        minWidth: 140,

                        formatter: function ( cell )
                        {
                            const value = cell.getValue();

                            if ( !value )
                                return "N/A";

                            return value.substring( 0, 10 );
                        }
                    }
                ]
        } );
}


function openLifeUtilizationStageDrawer ()
{
    $( "#lifeUtilizationStageOverlay" ).removeClass( "hidden" );

    requestAnimationFrame( () =>
    {
        $( "#lifeUtilizationStageDrawer" )
            .removeClass( "-translate-x-full" );
    } );
}


function closeLifeUtilizationStageDrawer ()
{
    $( "#lifeUtilizationStageDrawer" )
        .addClass( "-translate-x-full" );

    setTimeout( () =>
    {
        $( "#lifeUtilizationStageOverlay" )
            .addClass( "hidden" );
    }, 300 );
}


$( "#btnCloseLifeUtilizationStageDrawer" ).on( "click", function ()
{
    closeLifeUtilizationStageDrawer();
} );


$( "#lifeUtilizationStageOverlay" ).on( "click", function ()
{
    closeLifeUtilizationStageDrawer();
} );