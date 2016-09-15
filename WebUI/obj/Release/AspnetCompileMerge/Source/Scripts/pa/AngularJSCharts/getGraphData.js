var app = angular.module("myApp", []);
app.service('GraphInfo', function ($http, $rootScope) {
    alert("Witnessing service");

    $http({
        method: 'GET',
        url: '/Project/EffortVariance?type=1',
    }).then(function (response) {
        $rootScope.graphData.GraphType = response.data.GraphType;
        $rootScope.graphData.overallEffVariance = response.overallEffVariance,
        $rootScope.graphData.overallSchVariance = response.overallSchVariance,
        $rootScope.graphData.LCL = response.LCL,
        $rootScope.graphData.UCL = response.UCL,
        $rootScope.graphData.Labels = response.Labels,
        $rootScope.graphData.Series = response.Series,
        $rootScope.graphData.Data = response.Data,
        $rootScope.graphData.Title = response.Title
    }, function (response) {
        alert("Error in Line Graph");
    });
    alert("Defining Ang JS Service Method");

    this.loadData = function (type) {
        $http({
            method: 'GET',
            url: '/Project/EffortVariance?type=' + type,
        }).then(function (response) {
            $rootScope.graphData.GraphType = response.data.GraphType;
            $rootScope.graphData.overallEffVariance = response.overallEffVariance,
            $rootScope.graphData.overallSchVariance = response.overallSchVariance,
            $rootScope.graphData.LCL = response.LCL,
            $rootScope.graphData.UCL = response.UCL,
            $rootScope.graphData.Labels = response.Labels,
            $rootScope.graphData.Series = response.Series,
            $rootScope.graphData.Data = response.Data,
            $rootScope.graphData.Title = response.Title
        }, function (response) {
            alert("Error in Line Graph");
        });
    }

    this.data = function () {
        return $rootScope.graphData;
    }
});
