var varianceGraphComponent = {};
var OvlEffInstance = {};
var OvlSchInstance = {};
angular.module("myApp", ['dx']).controller('Graphs', function ($scope, $http)
{
    $http({
        method: 'GET',
        url: '/Project/EffortVariance/type=1',
    }).then(function (response) {
        $scope.effGaugeSettings = {
            preset: "preset2",
            scale: {
                startValue: -3,
                endValue: 3,
                majorTick: {
                    tickInterval: 1
                }
            },
            subvalueIndicator: {
                type: 'triangleMarker',
                color: 'forestgreen',
                length: 20
            },
            subvalues: [response.data.LCL, response.data.UCL],
            rangeContainer: {
                backgroundColor: "none",
                ranges: [
                    {
                        startValue: response.data.LCL - 3,
                        endValue: response.data.LCL,
                        color: "blue"
                    },
                    {
                        startValue: response.data.LCL,
                        endValue: response.data.UCL,
                        color: "green"
                    },
                    {
                        startValue: response.data.UCL,
                        endValue: response.data.UCL + 2,
                        color: "orange"
                    },
                    {
                        startValue: response.data.UCL,
                        endValue: response.data.UCL + 3,
                        color: "red"
                    },
                ]
            },

            value: response.data.overallEffVariance,

            onInitialized: function (e) {
                OvlEffInstance = e.component;
            }
        };
        OvlEffInstance.value = response.data.overallEffVariance;

        $scope.schGaugeSettings = {
            preset: "preset2",
            scale: {
                startValue: -3,
                endValue: 3,
                majorTick: {
                    tickInterval: 1
                }
            },
            subvalueIndicator: {
                    type: 'triangleMarker',
                    color: 'forestgreen',
                    length: 20
            },
            subvalues: [response.data.LCL, response.data.UCL],
            rangeContainer: {
                backgroundColor: "none",
                ranges: [
                    {
                        startValue: response.data.LCL - 3,
                        endValue: response.data.LCL,
                        color: "blue"
                    },
                    {
                        startValue: response.data.LCL,
                        endValue: response.data.UCL,
                        color: "green"
                    },
                    {
                        startValue: response.data.UCL,
                        endValue: response.data.UCL + 2,
                        color: "orange"
                    },
                    {
                        startValue: response.data.UCL,
                        endValue: response.data.UCL + 3,
                        color: "red"
                    },
                ]
            },

            value: response.data.overallSchVariance,

            onInitialized: function (e) {
                OvlSchInstance = e.component;
            }
        };


        $scope.varianceGraphSettings = {
            dataSource: response.data.data,
            commonSeriesSettings: {
                argumentField: response.data.argumentField
            },
            series: response.data.series,
            argumentAxis: {
                grid: {
                    visible: true
                }
            },
            tooltip: {
                enabled: true
            },
            legend: {
                visible: false
            },
            commonPaneSettings: {
                border: {
                    visible: true,
                    right: false
                }
            },
            legend: {
                verticalAlignment: 'top',
                horizontalAlignment: 'right'
            },
            onDrawn: function (e) {
                varianceGraphComponent = e.component;
            }
        };
        $scope.VarianceType = "1";
    }, function (response) {
        alert("Error in Line Graph");
    });

    $scope.ReloadVariance = function () {
        $http({
            method: 'GET',
            url: '/Project/EffortVariance?type=' + $scope.VarianceType,
        }).then(function (response) {
            varianceGraphComponent.option('dataSource',response.data.data);
        }, function (response) {
            alert("Error in Line Graph");
        });
    };

    // Load Pie Graph
    $http({
        method: 'GET',
        url: '/Project/EmployeePieChart',
    }).then(function (response) {
        $scope.pieChartSettings = {
            size:{ 
                width: 500
            },
            dataSource: response.data.data,
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
                    argumentField: response.data.argumentField,
                    valueField: response.data.valueField,
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
        };
    });

    // Load Bar Graph
    $http({
        method: 'GET',
        url: '/Project/ProjectComplianceChart',
    }).then(function (response) {
        $scope.barChartSettings = {
            dataSource: response.data.data,
            tooltip: {
                enabled: true,
                percentPrecision: 2,
            },
            series: [
                {
                    type: "bar",
                    argumentField: response.data.argumentField,
                    valueField: response.data.valueField,
                    name: "PCI Compliance",
                    color: "#30abe0"
                }
            ],
            legend: {
                visible: false
            },
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
        };
    });

});


