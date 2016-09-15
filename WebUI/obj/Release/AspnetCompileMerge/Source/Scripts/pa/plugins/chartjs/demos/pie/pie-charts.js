$(function (){
	
/**********************************
Pie Charts
**********************************/
var PCAllocation = [
    { country: "> 100 %", area: 50 },
    { country: "100 %", area: 20 },
    { country: "80 - 99 %", area: 5 },
    { country: "50 - 79 %", area: 2 },
    { country: "< 50 %", area: 3 },
    { country: "Not Allocated", area: 20 },
];

var Billable = [
    { country: "Billable", area: 40 },
    { country: "Non-Billable", area: 29 },
    { country: "Support", area: 11 },
    { country: "Not Allocated", area: 20 },
];

var PrjBillable = [
    { country: "Billable", area: 15 },
    { country: "Non-Billable", area: 6 }
];

var PrjPCAllocation = [
{ country: "> 100 %", area: 2 },
{ country: "100 %", area: 6 },
{ country: "80 - 99 %", area: 10 },
{ country: "50 - 79 %", area: 2 }
];

var PrjEmployees = [
{ country: "COE", area: 5 },
{ country: "Qmply", area: 6 },
{ country: "BPL Underwriting", area: 2 },
{ country: "DB Migration", area: 8 }
];

$("#PieChart").dxPieChart({
    size: {
        width: 500
    },
    dataSource: PCAllocation,
    tooltip: {
        enabled: true,
        percentPrecision: 2,
        customizeText: function () {
            return this.percentText + " (" + (this.argumentText == "Not Allocated" ? "" : "Allocated ") + this.argumentText + ")";
        }
    },
    series: [
        {
            type: "doughnut",
            argumentField: "country",
            valueField: "area",
            label: {
                visible: true,
                connector: {
                    visible: true,
                    width: 1
                }
            }
        }
    ],
    loadingIndicator: {
        show: true,
        backgroundColor: 'lightcyan',
        font: {
            weight: 700,
            size: 16
        }
    },
    animation: {
        duration: 3000,
        easing: 'linear'
    }
});



var OrgLevel = [
{ country: "BNFS", area: 23 },
{ country: "Retail", area: 17 },
{ country: "Insurance", area: 49 },
{ country: "Support", area: 11 },
];

$("#PieChart").dxPieChart({
    size:{ 
        width: 500
    },
    dataSource: PCAllocation,
    tooltip: {
        enabled: true,
        percentPrecision: 2,
        customizeText: function () {
            return this.percentText + " (" + (this.argumentText == "Not Allocated" ? "" : "Allocated ") + this.argumentText + ")";
        }
    },
    series: [
        {
            type: "doughnut",
            argumentField: "country",
            valueField: "area",
            label:{
                visible: true,
                connector:{
                    visible:true,           
                    width: 1
                }
            }
        }
    ],
    loadingIndicator: {
        show: true,
        backgroundColor: 'lightcyan',
        font: {
            weight: 700,
            size: 16
        }
    },
    animation: {
        duration: 3000,
        easing: 'linear'
    }
});


$("body").delegate(".changeGraph", "change", function () {
    var selectedID = $(this).find("option:selected").eq(0).val();
    var piechart = $("#PieChart").dxPieChart("instance");
    switch (selectedID) {
        case "1":
            piechart.option({ dataSource: PCAllocation });
            break;
        case "2":
            piechart.option({ dataSource: Billable });
            break;
        case "3":
            piechart.option({ dataSource: OrgLevel });
            break;
        case "4":
            piechart.option({ dataSource: PrjPCAllocation });
            break;
        case "5":
            piechart.option({ dataSource: PrjBillable });
            break;
        case "6":
            piechart.option({ dataSource: PrjEmployees });
            break;
        default:
            piechart.option({ dataSource: PCAllocation });
            break;
    }
});


/**********************************
Doughnut Charts
**********************************/
var DoughnutChartdataSource = [
    {region: "Asia", val: 4119626293},
    {region: "Africa", val: 1012956064},
    {region: "Northern America", val: 344124520},
    {region: "Latin America", val: 590946440},
    {region: "Europe", val: 727082222},
    {region: "Oceania", val: 35104756}
];

$("#DoughnutChart").dxPieChart({
    dataSource: DoughnutChartdataSource,
	tooltip: {
		enabled: true,
		format:"millions",
		percentPrecision: 2,
		customizeText: function() { 
			return this.valueText + " - " + this.percentText;
		}
	},
	legend: {
		horizontalAlignment: "right",
		verticalAlignment: "top",
		margin: 0
	},
	series: [{
		type: "doughnut",
		argumentField: "region",
		label: {
			visible: true,
			format: "millions",
			connector: {
				visible: true
			}
		}
	}]
});


});