# Base Components Usage Guide

## 📦 New Utility Components

This guide covers the three new utility components created to standardize common UI patterns across the application.

---

## 1. BaseCard

A versatile card component for wrapping content with consistent styling.

### Basic Usage

```razor
<BaseCard Title="User Information" Icon="bi bi-person">
    <BodyContent>
        <p>Your content here</p>
    </BodyContent>
</BaseCard>
```

### Advanced Usage

```razor
<BaseCard Title="Dashboard Stats" 
          Subtitle="Last 7 days"
          Icon="bi bi-graph-up"
          Hoverable="true"
          Variant="elevated">
    <ActionContent>
        <BaseButton Variant="outline-primary" Size="sm">View All</BaseButton>
    </ActionContent>
    <BodyContent>
        <!-- Stats content -->
    </BodyContent>
    <FooterContent>
        <small class="text-muted">Updated 5 minutes ago</small>
    </FooterContent>
</BaseCard>
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | string | null | Card title text |
| `Subtitle` | string | null | Optional subtitle below title |
| `Icon` | string | null | Bootstrap icon class |
| `HeaderContent` | RenderFragment | null | Custom header content (overrides Title) |
| `BodyContent` | RenderFragment | null | Main card content |
| `FooterContent` | RenderFragment | null | Footer content |
| `ActionContent` | RenderFragment | null | Action buttons in header |
| `Hoverable` | bool | false | Enable hover elevation effect |
| `Clickable` | bool | false | Make entire card clickable |
| `NoBorder` | bool | false | Remove header/footer borders |
| `NoPadding` | bool | false | Remove body padding |
| `Padding` | string | "md" | Body padding size (sm, md, lg, none) |
| `Variant` | string | "default" | Card style (default, outlined, elevated, flat) |
| `OnClick` | EventCallback | - | Click handler (requires Clickable=true) |

### Variants

```razor
<!-- Default Card -->
<BaseCard Title="Default">...</BaseCard>

<!-- Outlined Card -->
<BaseCard Title="Outlined" Variant="outlined">...</BaseCard>

<!-- Elevated Card (more shadow) -->
<BaseCard Title="Elevated" Variant="elevated">...</BaseCard>

<!-- Flat Card (no shadow, light background) -->
<BaseCard Title="Flat" Variant="flat">...</BaseCard>
```

### Use Cases

✅ **Dashboard Widgets**
```razor
<BaseCard Title="Active Users" Icon="bi bi-people" Variant="elevated">
    <BodyContent>
        <h2>1,234</h2>
        <p class="text-success">+12% vs last week</p>
    </BodyContent>
</BaseCard>
```

✅ **Forms**
```razor
<BaseCard Title="Update Profile" Icon="bi bi-pencil">
    <BodyContent>
        <BaseInput Label="Name" @bind-Value="name" />
        <BaseInput Label="Email" @bind-Value="email" />
    </BodyContent>
    <FooterContent>
        <BaseButton Variant="primary">Save Changes</BaseButton>
    </FooterContent>
</BaseCard>
```

✅ **Lists**
```razor
<BaseCard Title="Recent Activity" Icon="bi bi-clock-history">
    <ActionContent>
        <BaseButton Variant="link" Size="sm">See All</BaseButton>
    </ActionContent>
    <BodyContent>
        <!-- Activity list -->
    </BodyContent>
</BaseCard>
```

---

## 2. BaseEmptyState

A component for displaying "no data" states with style and animation.

### Basic Usage

```razor
<BaseEmptyState 
    Icon="bi bi-inbox"
    Title="No Items Found"
    Message="There are no items to display at this time" />
```

### With Actions

```razor
<BaseEmptyState 
    Icon="bi bi-search"
    Title="No Search Results"
    Message="We couldn't find any tickets matching your search criteria">
    <ActionContent>
        <BaseButton Variant="primary" Icon="bi bi-plus">
            Create New Ticket
        </BaseButton>
    </ActionContent>
</BaseEmptyState>
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Icon` | string | "bi bi-inbox" | Bootstrap icon class |
| `Title` | string | "No Data Found" | Main title text |
| `Message` | string | null | Descriptive message |
| `IconContent` | RenderFragment | null | Custom icon content |
| `ActionContent` | RenderFragment | null | Action buttons/links |
| `Size` | string | "default" | Size variant (compact, default, spacious) |
| `IconVariant` | string | "default" | Icon color (default, primary, success, warning, danger, info) |

### Icon Variants

```razor
<!-- Primary (blue) -->
<BaseEmptyState Icon="bi bi-search" IconVariant="primary" />

<!-- Success (green) -->
<BaseEmptyState Icon="bi bi-check-circle" IconVariant="success" />

<!-- Warning (orange) -->
<BaseEmptyState Icon="bi bi-exclamation-triangle" IconVariant="warning" />

<!-- Danger (red) -->
<BaseEmptyState Icon="bi bi-x-circle" IconVariant="danger" />

<!-- Info (blue) -->
<BaseEmptyState Icon="bi bi-info-circle" IconVariant="info" />
```

### Use Cases

✅ **Empty Tables**
```razor
@if (!tickets.Any())
{
    <BaseEmptyState 
        Icon="bi bi-ticket"
        Title="No Tickets"
        Message="There are no tickets for the selected period">
        <ActionContent>
            <BaseButton Variant="primary" OnClick="RefreshData">
                Refresh
            </BaseButton>
        </ActionContent>
    </BaseEmptyState>
}
```

✅ **Search Results**
```razor
<BaseEmptyState 
    Icon="bi bi-search"
    IconVariant="info"
    Title="No results found"
    Message="Try adjusting your search criteria" />
```

✅ **No Assignments**
```razor
<BaseEmptyState 
    Icon="bi bi-person-slash"
    IconVariant="warning"
    Title="No Staff Assigned"
    Message="This counter has no staff assigned">
    <ActionContent>
        <BaseButton Variant="primary" Icon="bi bi-person-plus">
            Assign Staff
        </BaseButton>
    </ActionContent>
</BaseEmptyState>
```

---

## 3. BaseLoadingSpinner

A flexible loading indicator with multiple animation styles.

### Basic Usage

```razor
<BaseLoadingSpinner LoadingText="Loading data..." />
```

### Variants

```razor
<!-- Default Spinner -->
<BaseLoadingSpinner Variant="spinner" />

<!-- Animated Dots -->
<BaseLoadingSpinner Variant="dots" LoadingText="Processing..." />

<!-- Pulse Animation -->
<BaseLoadingSpinner Variant="pulse" ShowText="false" />

<!-- Animated Bars -->
<BaseLoadingSpinner Variant="bars" Color="success" />
```

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | string | "spinner" | Animation type (spinner, dots, pulse, bars) |
| `Size` | string | "md" | Size (sm, md, lg) |
| `Color` | string | "primary" | Color theme (primary, success, danger, warning, info) |
| `LoadingText` | string | "Loading..." | Text to display |
| `ShowText` | bool | true | Show/hide loading text |
| `Fullscreen` | bool | false | Cover entire screen |
| `Inline` | bool | false | Display inline (no padding) |

### Fullscreen Overlay

```razor
@if (isLoading)
{
    <BaseLoadingSpinner 
        Fullscreen="true"
        Variant="pulse"
        LoadingText="Please wait..." />
}
```

### Inline Loading

```razor
<BaseButton Disabled="@isLoading">
    @if (isLoading)
    {
        <BaseLoadingSpinner Inline="true" Size="sm" ShowText="false" />
    }
    else
    {
        <span>Submit</span>
    }
</BaseButton>
```

### Use Cases

✅ **Page Loading**
```razor
@if (data == null)
{
    <BaseLoadingSpinner 
        Variant="spinner"
        Size="lg"
        LoadingText="Loading dashboard..." />
}
else
{
    <!-- Display data -->
}
```

✅ **Data Processing**
```razor
@if (isProcessing)
{
    <BaseLoadingSpinner 
        Variant="bars"
        Color="success"
        LoadingText="Processing your request..." />
}
```

✅ **Button Loading State**
```razor
<BaseButton Variant="primary" Disabled="@isSaving">
    @if (isSaving)
    {
        <BaseLoadingSpinner Inline="true" Size="sm" Variant="dots" ShowText="false" />
        <span class="ms-2">Saving...</span>
    }
    else
    {
        <span>Save Changes</span>
    }
</BaseButton>
```

---

## 🎯 Combined Examples

### Dashboard Card with Loading

```razor
<BaseCard Title="Recent Activity" Icon="bi bi-clock-history" Variant="elevated">
    <BodyContent>
        @if (activities == null)
        {
            <BaseLoadingSpinner Variant="dots" LoadingText="Loading activities..." />
        }
        else if (!activities.Any())
        {
            <BaseEmptyState 
                Icon="bi bi-activity"
                IconVariant="info"
                Title="No Recent Activity"
                Size="compact" />
        }
        else
        {
            @foreach (var activity in activities)
            {
                <!-- Display activity -->
            }
        }
    </BodyContent>
</BaseCard>
```

### Table with Empty State

```razor
<BaseCard Title="User List" Icon="bi bi-people">
    <ActionContent>
        <BaseButton Variant="primary" Icon="bi bi-plus">Add User</BaseButton>
    </ActionContent>
    <BodyContent>
        @if (isLoading)
        {
            <BaseLoadingSpinner />
        }
        else if (!users.Any())
        {
            <BaseEmptyState 
                Icon="bi bi-person-slash"
                Title="No Users Found"
                Message="Get started by adding your first user">
                <ActionContent>
                    <BaseButton Variant="primary" Icon="bi bi-plus">
                        Add User
                    </BaseButton>
                </ActionContent>
            </BaseEmptyState>
        }
        else
        {
            <BaseTable>
                <!-- User table -->
            </BaseTable>
        }
    </BodyContent>
</BaseCard>
```

---

## 🎨 Best Practices

### BaseCard
- ✅ Use for grouping related content
- ✅ Add `Hoverable` for clickable items in a grid
- ✅ Use `Variant="elevated"` for important cards
- ❌ Don't nest cards too deeply (max 2 levels)

### BaseEmptyState
- ✅ Always provide an action when possible
- ✅ Use appropriate icon variant for context
- ✅ Keep messages concise and helpful
- ❌ Don't use for temporary loading states

### BaseLoadingSpinner
- ✅ Use `Fullscreen` for page-level loading
- ✅ Use `Inline` for button loading states
- ✅ Match color to action (success for save, primary for load)
- ❌ Don't show multiple fullscreen spinners

---

## 📊 Component Matrix

| Scenario | Component | Configuration |
|----------|-----------|---------------|
| Dashboard widget | BaseCard | `Variant="elevated"` |
| Form section | BaseCard | `Title` + `FooterContent` |
| Empty table | BaseEmptyState | With `ActionContent` |
| No search results | BaseEmptyState | `IconVariant="info"` |
| Page loading | BaseLoadingSpinner | `Size="lg"` |
| Processing data | BaseLoadingSpinner | `Variant="bars"` |
| Button saving | BaseLoadingSpinner | `Inline="true"` + `Size="sm"` |

---

**Last Updated:** 2025-11-29  
**Components Version:** 1.0.0
