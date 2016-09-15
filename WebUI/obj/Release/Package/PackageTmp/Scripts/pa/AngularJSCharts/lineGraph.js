alert("Running Line Graph");
angular.module("myApp", ["chart.js"]).controller("LineCtrl", function ($scope, GraphInfo) {
    alert("Attempting line graph");
    GraphInfo.loadData(1);
    var data = GraphInfo.data;
    $scope.VarianceType = 1;
    $scope.labels = data.Labels;
    $scope.series = data.Series;
    $scope.data = data.Data;

    $scope.ReloadVariance = function () {
        GraphInfo.loadData();
        var data = GraphInfo.data;
        $scope.VarianceType = 1;
        $scope.labels = data.Labels;
        $scope.series = data.Series;
        $scope.data = data.Data;    
    };
});


