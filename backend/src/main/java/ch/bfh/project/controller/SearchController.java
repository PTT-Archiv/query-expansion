package ch.bfh.project.controller;

import ch.bfh.project.model.ResponseSummary;
import ch.bfh.project.model.SearchSettings;
import ch.bfh.project.service.SearchService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/search")
@RequiredArgsConstructor
@CrossOrigin("http://localhost:4200")
public class SearchController {

	private final SearchService service;

	/**
	 * This REST-endpoint allows to make a PTT-search with a searchTerm that can optionally be expanded.
	 *
	 * @param searchTerm the searchTerm that should be searched
	 * @param searchSettings contains the optional settings of expansion
	 * @return {@link ResponseSummary} contains the expanded keywords and the sorted search results
	 */
	@PostMapping(consumes = "application/json")
	public ResponseEntity<ResponseSummary> search(@RequestParam("searchTerm") String searchTerm, @RequestBody(required = false) SearchSettings searchSettings) {
		HttpHeaders headers = new HttpHeaders();
		return ResponseEntity.ok().headers(headers).body(service.search(searchTerm, searchSettings));
	}

}
