google.load('visualization', '1', {'packages': ['geochart']});
     google.setOnLoadCallback(drawRegionsMap);

      function drawRegionsMap() {
        var data = google.visualization.arrayToDataTable([
          ['Country', 'Popularity'],
          ['Germany', 200],
          ['United States', 300],
          ['Brazil', 400],
          ['Canada', 500],
          ['France', 600],
          ['RU', 700]
        ]);

        var options = {
		height: 500};

        var chart = new google.visualization.GeoChart(document.getElementById('world_map'));
        chart.draw(data, options);
    };
	
	google.setOnLoadCallback(drawMarkersMap);

	function drawMarkersMap() {
      var data = google.visualization.arrayToDataTable([
        ['City',   'Population', 'Area'],
        ['Rome',      2761477,    1285.31],
        ['Milan',     1324110,    181.76],
        ['Naples',    959574,     117.27],
        ['Turin',     907563,     130.17],
        ['Palermo',   655875,     158.9],
        ['Genoa',     607906,     243.60],
        ['Bologna',   380181,     140.7],
        ['Florence',  371282,     102.41],
        ['Fiumicino', 67370,      213.44],
        ['Anzio',     52192,      43.43],
        ['Ciampino',  38262,      11]
      ]);

      var options = {
        region: 'IT',
        displayMode: 'markers',
        colorAxis: {colors: ['#66c88d', '#f06060']}
      };

      var chart = new google.visualization.GeoChart(document.getElementById('marker_map'));
      chart.draw(data, options);
    };
	
	
   google.setOnLoadCallback(drawUsMap);

    function drawUsMap() {
      var data = google.visualization.arrayToDataTable([
        ['City', 'Popularity'],
        ['New York', 200],
        ['Boston', 300],
        ['Miami', 400],
        ['Chicago', 500],
        ['Los Angeles', 600],
        ['Houston', 700]
      ]);

      var options = {
        region: 'US',
        displayMode: 'markers',
        colorAxis: {colors: ['#66c88d', '#f06060']}
      };

      var chart = new google.visualization.GeoChart(document.getElementById('us_map'));
      chart.draw(data, options);
    };