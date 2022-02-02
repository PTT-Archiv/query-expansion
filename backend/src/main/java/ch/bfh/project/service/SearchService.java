package ch.bfh.project.service;

import ch.bfh.project.model.ResponseSummary;
import ch.bfh.project.model.SearchEntry;
import ch.bfh.project.model.SearchSettings;
import ch.bfh.project.repository.HistoricalRepository;
import ch.bfh.project.repository.LocalityRepository;
import lombok.RequiredArgsConstructor;
import org.apache.commons.lang3.StringUtils;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.Collection;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@Service
@RequiredArgsConstructor
public class SearchService {

    private final PTTArchivService pttArchivService;
    private final LocalityRepository localityRepository;
    private final HistoricalRepository historicalRepository;

    /**
     * The search method expands the search term depending on the settings and calls the PTT-Archive Service to perform the search.
     * It removes duplicates if there are any and sorts the search entries.
     * It also counts the amount of search entries per search term (including expanded search terms).
     *
     * @param searchTerm the given searchTerm
     * @param searchSettings contains the optional settings of expansion
     * @return {@link ResponseSummary} contains the expanded searchTerms and the sorted search results
     */
    public ResponseSummary search(String searchTerm, SearchSettings searchSettings) {
        Map<String, Integer> entriesPerSearchTerm = new HashMap<>();
        // input term search
        List<SearchEntry> searchEntries = pttArchivService.query(searchTerm);
        entriesPerSearchTerm.put(searchTerm, 0);
        if (searchSettings != null) {
            // localities search
            Set<Integer> localitiesId = new HashSet<>();
            Set<SearchEntry> localitiesSearchEntries = new HashSet<>();
            Set<SearchEntry> historicalSearchEntries = new HashSet<>();
            localityRepository.findLocalitiesByKeyword(searchTerm).forEach(l -> {
                if (searchSettings.isTranslatedLocalityNames()) {
                    List.of(l.getDe(), l.getFr(), l.getIt())
                            .forEach(e ->
                                    expandedSearch(removeNumbersFromLocality(e), localitiesSearchEntries, entriesPerSearchTerm));
                }
                localitiesId.add(l.getId()); // The historical name matches are mapped with the locality ids in the database
            });
            // historical search
            if (searchSettings.isHistoricalLocalityNames()) {
                historicalRepository.findByLocalitiesIds(localitiesId)
                        .forEach(id -> Set.of(id.getNames().split(";"))
                                .forEach(h -> expandedSearch(h, historicalSearchEntries, entriesPerSearchTerm)));
            }
            // aggregate search entries and remove duplicates
            searchEntries = Stream.of(searchEntries, localitiesSearchEntries, historicalSearchEntries) // the different search entry sources are added in the wanted order into the stream
                            .flatMap(Collection::stream)
                            .distinct()
                            .collect(Collectors.toList()); // the sort order is kept due to the list data structure
        }
        // counting entries per search term
        searchEntries.forEach(searchEntry ->
                entriesPerSearchTerm.put(searchEntry.getFirstMatchingSearchTerm(),
                        entriesPerSearchTerm.get(searchEntry.getFirstMatchingSearchTerm()) + 1));
        return new ResponseSummary(entriesPerSearchTerm, searchEntries);
    }

    private void expandedSearch(String searchTerm, Set<SearchEntry> searchEntries,
                                Map<String, Integer> entriesPerSearchTerm) {
        if (!searchTerm.isEmpty()) {
            entriesPerSearchTerm.putIfAbsent(searchTerm, 0);
            searchEntries.addAll(pttArchivService.query(searchTerm));
        }
    }

    private String removeNumbersFromLocality(String locality) {
        return locality.replaceAll("\\d", "").trim();
    }
}
