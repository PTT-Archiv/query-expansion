package ch.bfh.project.model;

import lombok.Data;

import java.util.Objects;

@Data
public class SearchEntry {

    /**
     * The entry is the url leading to the PTT-Record.
     */
    private final String entry;
    /**
     * The search is performed via the name field.
     */
    private final String name;
    /**
     * A short description of the PTT-Record.
     */
    private final String scopeAndContent;
    /**
     * Shows the search term that matched, and if several matched, shows only the first one.
     */
    private final String firstMatchingSearchTerm;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        SearchEntry that = (SearchEntry) o;
        return Objects.equals(entry, that.entry);
    }

    @Override
    public int hashCode() {
        return Objects.hash(entry);
    }
}
