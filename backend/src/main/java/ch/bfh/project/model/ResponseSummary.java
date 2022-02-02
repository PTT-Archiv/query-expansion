package ch.bfh.project.model;

import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class ResponseSummary {

    /**
     * Contains the different search terms that were expanded and the number of search entries they matched.
     */
    private final Map<String, Integer> extendedSearchTerms;
    /**
     * Contains the matched search entries.
     */
    private final List<SearchEntry> searchEntries;
}
