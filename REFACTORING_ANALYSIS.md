# Component Refactoring Analysis

## Summary
This document identifies opportunities to refactor existing pages to use the new base components from `/Components/Common/`.

---

## 🎯 Refactoring Opportunities by Page

### 1. **CounterManagement.razor** (HIGH PRIORITY)
**Current Issues:**
- Custom modal implementation (lines 345-436)
- Custom buttons without standardized styling
- Custom checkboxes for service selection (line 671-674)
- Custom toggle switches (lines 269-296)
- Status badges with inline styles (lines 497-511)

**Recommended Changes:**
- ✅ Replace custom modal with `<BaseModal>`
- ✅ Replace assign/unassign buttons with `<BaseButton>`
- ✅ Replace service checkboxes with `<BaseCheckbox>`
- ✅ Use `<StatusBadge>` for counter status display
- ✅ Replace refresh button with `<BaseButton>`
- ✅ Use `<UserAvatar>` for assigned teller display (lines 521-523)
- ✅ Use `<BaseAlert>` for warning messages (lines 622-625)

**Impact:** High - Will significantly reduce code and improve consistency

---

### 2. **StaffManagement.razor**
**File Location:** `/Pages/TM/StaffManagement.razor`

**Likely Issues:**
- Similar modal patterns to CounterManagement
- User listing tables
- Form inputs for user management

**Recommended Changes:**
- ✅ Use `<BaseTable>` for staff listing
- ✅ Use `<BaseModal>` for add/edit dialogs
- ✅ Use `<BaseInput>` for form fields
- ✅ Use `<BaseButton>` for actions
- ✅ Use `<UserAvatar>` for user display
- ✅ Use `<PageHeader>` for page title

---

### 3. **ServiceManagement.razor**
**File Location:** `/Pages/TM/ServiceManagement.razor`

**Likely Issues:**
- Service form inputs
- Service listing table
- Status toggles
- Modal dialogs

**Recommended Changes:**
- ✅ Use `<BaseTable>` with pagination for service list
- ✅ Use `<BaseModal>` for add/edit service
- ✅ Use `<BaseInput>` for service name, description
- ✅ Use `<BaseCheckbox>` for IsActive toggle
- ✅ Use `<BaseButton>` for actions
- ✅ Use `<BaseAlert>` for validation messages

---

### 4. **Counter/Dashboard.razor** (MEDIUM PRIORITY)
**Current State:** Already uses `CustomerInfoForm` and `TicketDisplay`

**Additional Changes Needed:**
- ✅ Use `<BaseButton>` for action buttons
- ✅ Use `<StatusBadge>` for ticket status
- ✅ Use `<BaseModal>` for transfer/cancel dialogs
- ✅ Use `<BaseAlert>` for error/success messages
- ✅ Use `<BaseToast>` for notifications

---

### 5. **Kiosk Pages** (MEDIUM PRIORITY)

#### a. **Kiosk/Index.razor**
**Recommended Changes:**
- ✅ Use `<PageHeader>` for welcome screen
- ✅ Use `<BaseButton>` for language selection
- ✅ Use `<BaseCard>` wrapper (if we create one)

#### b. **Kiosk/Services.razor**
**Recommended Changes:**
- ✅ Use `<BaseButton>` for service selection buttons
- ✅ Use `<PageHeader>` for title
- ✅ Use `<BaseAlert>` for instructions

#### c. **Kiosk/TicketCreated.razor**
**Recommended Changes:**
- ✅ Use `<PageHeader>` for success message
- ✅ Use `<BaseButton>` for "Take Another Ticket"
- ✅ Use `<BaseTruncate>` for long service names

---

### 6. **BranchDisplay/BranchDisplay.razor** (LOW PRIORITY)
**Purpose:** Public display screen for branch queue

**Recommended Changes:**
- ✅ Use `<BaseTable>` for ticket queue display
- ✅ Use `<StatusBadge>` for ticket status
- ✅ Use `<BaseTruncate>` for long names
- Keep large display styling as-is (specialized use case)

---

### 7. **Counter/Counter-Display.razor** (LOW PRIORITY)
**Purpose:** Counter-specific display screen

**Recommended Changes:**
- ✅ Use `<StatusBadge>` for counter status
- ✅ Use `<BaseInput>` for counter selection (setup mode)
- ✅ Use `<BaseButton>` for setup confirmation
- Keep display-specific styling for visibility

---

### 8. **Login.razor** (ALREADY REFACTORED)
**Status:** ✅ Complete
- Already uses `<BaseSelect>` and `<BaseButton>`
- Already uses `<UserAvatar>`

---

### 9. **User/Profile.razor**
**Recommended Changes:**
- ✅ Use `<PageHeader>` for profile title
- ✅ Use `<BaseInput>` for profile fields
- ✅ Use `<UserAvatar>` for profile picture
- ✅ Use `<BaseButton>` for save/cancel
- ✅ Use `<BaseAlert>` for success/error messages
- ✅ Use `<BaseModal>` for change password dialog

---

## 🎨 New Components to Create

Based on patterns found, consider creating:

1. **BaseCard.razor**
   - Wrapper for white rounded content cards
   - Consistent padding and shadow
   - Usage: Everywhere we see `.card` or `.counter-card` patterns

2. **BaseEmptyState.razor**
   - Icon + title + description for empty lists
   - Usage: Lines 426-436 in CounterManagement, similar in others

3. **BaseLoadingSpinner.razor**
   - Consistent loading spinner component
   - Usage: Lines 461-465, 472-476

---

## 📊 Refactoring Priority Matrix

### High Priority (Do First)
1. **CounterManagement.razor** - Most complex, biggest impact
2. **StaffManagement.razor** - Similar patterns
3. **ServiceManagement.razor** - Similar patterns

### Medium Priority (Do Next)
1. **Counter/Dashboard.razor** - User-facing, high usage
2. **Kiosk Pages** - Customer-facing, high visibility
3. **TMDashboard.razor** - Already partially done

### Low Priority (Can Wait)
1. **Display Pages** - Specialized styling needed
2. **Profile Page** - Low usage frequency

---

## ✅ Benefits of Refactoring

1. **Code Reduction**: ~30-40% less code per page
2. **Consistency**: Uniform look and feel
3. **Maintainability**: Fix once, apply everywhere
4. **Performance**: Optimized base components
5. **Accessibility**: Built-in ARIA labels and keyboard navigation
6. **Responsive**: All components are mobile-friendly

---

## 🚀 Implementation Plan

### Phase 1: TM Pages (Week 1)
- CounterManagement.razor
- StaffManagement.razor  
- ServiceManagement.razor

### Phase 2: Counter Pages (Week 2)
- Counter/Dashboard.razor
- Counter/Counter-Display.razor

### Phase 3: Kiosk Pages (Week 3)
- Kiosk/Index.razor
- Kiosk/Services.razor
- Kiosk/TicketCreated.razor

### Phase 4: Remaining Pages (Week 4)
- User/Profile.razor
- BranchDisplay/BranchDisplay.razor

---

## 📝 Notes

- All changes should be tested on multiple screen sizes
- Maintain existing functionality while improving code
- Update tests if components change significantly
- Document any breaking changes
