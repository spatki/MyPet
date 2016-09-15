try {
    var app = angular.module("myWFApp", ['localytics.directives']);
    app.controller('wf', function ($scope, $http, $timeout) {
        var _id = 1;
        var _dialogOpened = false;
        // Load workflow 
        $scope.wrkflw = {};
        $http.get('/Workflow/getWorkflowDetailsFor?functionID=' + $scope.functionID).success(function (wrkflowdata) {
            $scope.wrkflw = wrkflowdata.workflow;
            $scope.actions = wrkflowdata.actions;
            $scope.roles = wrkflowdata.role;
            $scope.status = wrkflowdata.status;
            $scope.usertype = wrkflowdata.workflow_user_type;
            $scope.workflow_direction = wrkflowdata.workflow_direction;
            $scope.emptyWf = wrkflowdata.emptyWorkflow;
            _id = wrkflowdata.newID;
        });
        var indexedWFs = [];

        $scope.wfFilter = function () {
            indexedWFs = [];
            return $scope.wrkflw;
        }

        $scope.filterWFs = function (wf) {
            var WFisNew = indexedWFs.indexOf(wf.PreStatusID) == -1;
            if (WFisNew) {
                indexedWFs.push(wf.PreStatusID);
            }
            return WFisNew;
        }

        $scope.showWF = function () {
            if ($scope.functionID == undefined || $scope.functionID == "") {
                $scope.wrkflw = undefined;
                $.pnotify({
                    title: 'Invalid Input',
                    text: "Pl. select a valid entry to display the workflow",
                    type: 'error'
                });
            }
            else {
                $http.get('/Workflow/getWorkflowDetailsFor?functionID=' + $scope.functionID).success(function (wrkflowdata) {
                    $scope.wrkflw = wrkflowdata.workflow;
                    $scope.actions = wrkflowdata.actions;
                    $scope.roles = wrkflowdata.role;
                    $scope.status = wrkflowdata.status;
                    $scope.usertype = wrkflowdata.workflow_user_type;
                    $scope.workflow_direction = wrkflowdata.workflow_direction;
                    $scope.emptyWf = wrkflowdata.emptyWorkflow;
                    _id = wrkflowdata.newID;
                });
            }
        }

        $scope.getAction = function (actionID) {
            for (a in $scope.actions) {
                if ($scope.actions[a].ID == actionID) {
                    return $scope.actions[a].Name;
                }
            }
        }

        $scope.getStatus = function (statusID) {
            switch (statusID) {
                case null:
                    return "New";
                    break;
                default:
                    for (a in $scope.status) {
                        if ($scope.status[a].ID == statusID) {
                            return $scope.status[a].Status;
                        }
                    }
                    break;
            }
        }

        $scope.getRole = function (roleID) {
            switch (roleID) {
                case null:
                    return "All Roles";
                    break;
                case 0:
                    return "Administrative Access";
                    break;
                default:
                    for (a in $scope.roles) {
                        if ($scope.roles[a].ID == roleID) {
                            return $scope.roles[a].LongName;
                        }
                    }
                    break;
            }
        }

        $scope.getUserType = function (usertypeID) {
            if (usertypeID != null) {
                for (a in $scope.usertype) {
                    if ($scope.usertype[a].Value == usertypeID) {
                        return $scope.usertype[a].Text;
                    }
                }
            }
            return "NA";
        }

        $scope.addData = function (ID)
        {
            alert("Mocking add");
        }

        $scope.startEditWF = function (ID) {
            if ($scope.functionID == undefined || $scope.functionID == "") {
                $.pnotify({
                    title: "Invalid Input",
                    text: "Pl. select a valid entry to display the workflow",
                    type: "error"
                });
                return;
            }
            $("#editRecord").modal("show");
            if (_dialogOpened == false) {
                specialDropDown(".cmb");
                _dialogOpened = true;
            }
            for (w in $scope.wrkflw) {
                if ($scope.wrkflw[w].ID == ID) {
                    $scope.newWF = angular.copy($scope.wrkflw[w]);
                    break;
                }
            }
            $scope.wfDetails();
            $timeout(function ()
            {
                $(".cmb").each(function (index, element) {
                    $(element).trigger("liszt:updated");
                });
            }, 1000);
        }

        $scope.saveWFDetails = function (dataValid) {
            if (dataValid) {
                $scope.newWF.UserName = $("#RoleID").find("option:selected").text();
                if ($scope.newWF.ID == 0) {
                    $scope.newWF.ID = _id++;
                }
                // Save this record on the server
                $http({
                    method: 'POST',
                    url: '/Workflow/saveWF',
                    data: $scope.newWF
                }).success(function (newData) {
                    // Search for the existing WF record
                    $scope.newWF = newData;
                    var found = false;
                    for (w in $scope.wrkflw) {
                        if ($scope.wrkflw[w].ID == $scope.newWF.ID) {
                            $scope.wrkflw[w] = $scope.newWF;
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        // This is a new record
                        $scope.wrkflw.push($scope.newWF);
                    }
                }).error(function (errMessage) {
                    $.pnotify({
                        title: 'Application Error',
                        text: "Pl. try again or contact the system administrator",
                        type: 'error'
                    });
                    return;
                });
                $("#editRecord").modal("hide");
                $.pnotify({
                    title: 'Saved Successfully',
                    type: 'info'
                });
            }
        }
        
        $scope.deleteWF = function (ID) {
            for (w in $scope.wrkflw) {
                if ($scope.wrkflw[w].ID == ID) {
                    $scope.newWF = angular.copy($scope.wrkflw[w]);
                    $("#deleteRecord").modal("show");
                    break;
                }
            }
        }

        $scope.deleteConfirm = function () {
            $http({
                method: 'POST',
                url: '/Workflow/deleteWF',
                data: $scope.newWF.RowIDs
            }).success(function (newData) {
                for (w in $scope.wrkflw) {
                    if ($scope.wrkflw[w].ID == $scope.newWF.ID) {
                        $scope.wrkflw.splice(w, 1);
                        $("#deleteRecord").modal("hide");
                        $.pnotify({
                            title: 'Deleted Successfully',
                            type: 'info'
                        });
                        break;
                    }
                }
            }).error(function (errMessage) {
                $.pnotify({
                    title: 'Application Error',
                    text: "Record not deleted",
                    type: 'error'
                });
            });
        }

        $scope.addNewWF = function () {
            if ($scope.functionID == undefined || $scope.functionID == "") {
                $.pnotify({
                    title: "Invalid Input",
                    text: "Pl. select a valid entry to display the workflow",
                    type: "error"
                });
                return;
            }
            $scope.newWF = angular.copy($scope.emptyWf);
            $("#editRecord").modal("show");
            if (_dialogOpened == false) {
                specialDropDown(".cmb");
                _dialogOpened = true;
            }
            $timeout(function () {
                $(".cmb").each(function (index, element) {
                    $(element).trigger("liszt:updated");
                });
            }, 1000);
        }

        $scope.wfDetails = function () {
            switch ($scope.newWF.RoleType) {
                case 1:   // Org Role
                case 7:   //  Project Role
                    // Additional detail is which role exactly
                    $http.get("/OrgRole/getListItemsJSON").success(function (data) {
                        $scope.wfDetailOptions = data;
                        $timeout(function() {
                            $("#RoleID").trigger("liszt:updated");
                            }, 1000);
                    });
                    break;
                case 9:   // Individual
                    // Additional detail is which individual exactly
                    $http.get('/Employee/getListItemsJSON').success(function (data) {
                        $scope.wfDetailOptions = data;
                        $timeout(function () {
                            $("#RoleID").trigger("liszt:updated");
                        }, 1000);
                    });
                    break;
                default:
                    $scope.wfDetailOptions = {};
                    $("#RoleID").trigger("liszt:updated");
                    break;
            }
        }

        $scope.UpdateName = function () {
            $scope.newWF.UserName = $scope.newWF.RoleID.name;
        }
    });

} catch (err) { alert("Custom Error: " + err.message); }
