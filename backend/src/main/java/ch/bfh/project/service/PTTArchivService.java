package ch.bfh.project.service;

import ch.bfh.project.model.SearchEntry;
import org.apache.jena.query.Query;
import org.apache.jena.query.QueryExecution;
import org.apache.jena.query.QueryExecutionFactory;
import org.apache.jena.query.QueryFactory;
import org.apache.jena.query.QuerySolution;
import org.apache.jena.query.ResultSet;
import org.apache.jena.query.Syntax;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class PTTArchivService {

	private static final String PTT_ARCHIV_URL = "https://data.ptt-archiv.ch/query";

	/**
	 * Querying the remote SPARQL service from PTT-Archiv
	 *
	 * @param searchTerm Search term for which a search is performed.
	 * @return the summarized search result, the set of all found entries
	 */
	public List<SearchEntry> query(String searchTerm) {
		List<SearchEntry> searchEntries = new ArrayList<>();

		String queryString = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" +
				"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" +
				"PREFIX rico: <https://www.ica.org/standards/RiC/ontology#>\n" +
				"\n" +
				"SELECT * \n" +
				"WHERE {\n" +
				"  GRAPH <http://example.org/graph/amos> {\n" +
				" \t?entry a rico:RecordSet.\n" +
				"    ?entry rico:name ?name.\n" +
				"    ?entry rico:scopeAndContent ?scopeAndContent.\n" +
				"    \n" +
				"    filter regex(?name, \"([^a-zA-Z])*" + searchTerm + "[^a-zA-Z]+\", \"i\")\n" +
				"  }\n" +
				"}";
		Query query = QueryFactory.create(queryString, Syntax.syntaxARQ);
		QueryExecution queryExecution = QueryExecutionFactory.sparqlService(PTT_ARCHIV_URL, query);
		ResultSet resultSet = queryExecution.execSelect();

		while (resultSet.hasNext()) {
			QuerySolution querySolution = resultSet.next();
			SearchEntry entry = new SearchEntry(
					querySolution.get("entry").toString(),
					querySolution.get("name").toString(),
					querySolution.get("scopeAndContent").toString(),
					searchTerm);
			searchEntries.add(entry);
		}

		queryExecution.close();

		return searchEntries;
	}
}
