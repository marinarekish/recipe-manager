# Demo scenarios (video script)

> Short clips for a portfolio walkthrough. Hosting is local-only; record the UI + (for login) API logs showing the one-time code when needed.
>
> **How to use:** each `▶ VIDEO` marker is a place to drop a short take. Keep clips ~15–60s unless noted.

---

## 0. Intro (optional, 20–30s)

**Talking points**

- Recipe Manager: Angular + ASP.NET Core + PostgreSQL
- Passwordless login (email code + JWT), recipes, favorites, roles (User / Administrator)

▶ VIDEO — app shell (sidebar + Explore) while you say the stack in one sentence

---

## 1. Auth — unknown email

**Goal:** request code for an email that is not registered.

**Steps**

1. Open Login
2. Enter unknown email → Request code
3. Show error / “no account” → link to Register

▶ VIDEO — login → request code → message that account was not found → navigate to Register

---

## 2. Auth — register

**Goal:** create a new user (default role: User).

**Steps**

1. Register: first name, last name, email, optional phone
2. Submit → success → go to Login (or verify flow if you chain it)

▶ VIDEO — fill register form → submit → confirmation / redirect to login

---

## 3. Auth — login with code (happy path)

**Goal:** request code → verify → land in the app.

**Steps**

1. Login: registered email → Request code
2. **Cut to** API console / terminal: show the 6-digit code (dev-only logging)
3. Verify screen: enter code → success
4. Land on Explore (or home) with sidebar user name

▶ VIDEO A — request code (UI only)  
▶ VIDEO B — terminal/log line with the code (blur other secrets)  
▶ VIDEO C — verify code → main app (Explore)

**Note on recording:** say clearly that production would email the code; in this demo the code is read from server logs.

---

## 4. Regular user — recipes

**Goal:** browse, open detail, manage **own** content only.

**Steps**

1. Explore: list all recipes (cards)
2. Open one recipe (detail)
3. My Recipes: only own recipes; Create recipe (short)
4. Optional: try edit/delete on **someone else’s** recipe → no manage actions / 403

▶ VIDEO — Explore → detail → My Recipes → create (or edit own)  
▶ VIDEO (optional) — other author’s recipe: no Edit/Delete for User

---

## 5. Regular user — favorites

**Goal:** add/remove favorites.

**Steps**

1. From Explore or detail: toggle favorite
2. Favorites page: list; unfavorite removes from list

▶ VIDEO — heart on → Favorites page → heart off / remove

---

## 6. Regular user — profile

**Goal:** update profile fields; email read-only.

**Steps**

1. Profile: change first/last/phone → Save
2. Sidebar name updates
3. Role badge shows **User**

▶ VIDEO — edit profile → save → sidebar reflects name

---

## 7. Logout

**Steps**

1. Logout → login screen
2. Optional: hit a protected URL → redirect to login

▶ VIDEO — logout → login page

---

## 8. Administrator — login

**Goal:** same passwordless flow; role **Administrator**.

**Steps**

1. Login as admin account (code from logs again if needed)
2. Sidebar shows **ADMIN** block (Users, Recipes, Lookups)

▶ VIDEO — admin login → sidebar with ADMIN section visible  
_(Skip full code ritual if already shown in §3; one quick “logged in as admin” is enough.)_

---

## 9. Administrator — users

**Goal:** list users, change role, delete (with last-admin protection).

**Steps**

1. Admin → Users
2. Change a user role User ↔ Administrator
3. Optional: delete a non-critical user (confirm)
4. Mention: system keeps at least one administrator

▶ VIDEO — users table → role dropdown → save effect  
▶ VIDEO (optional) — delete confirm or failed delete of last admin

---

## 10. Administrator — all recipes

**Goal:** see every recipe; delete any; no admin-edit required in demo.

**Steps**

1. Admin → Recipes
2. Search by title or author
3. Open detail via title
4. Delete one recipe (confirm)

▶ VIDEO — admin recipes table → search → delete

---

## 11. Administrator — lookups

**Goal:** manage categories / cuisines / ingredients; delete unused only.

**Steps**

1. Admin → Lookups
2. Switch tabs: Categories / Cuisines / Ingredients
3. Delete an unused item
4. Optional: delete in-use item → error (409), row remains

▶ VIDEO — tabs + delete unused  
▶ VIDEO (optional) — in-use delete error

---

## 12. Outro (optional)

**Talking points**

- Ownership + RBAC enforced on API and UI
- Passwordless auth, JWT, layered backend
- Next / not in demo: public hosting, real email delivery, form autocomplete (#60)

▶ VIDEO — freeze on Explore or architecture slide

---

## Suggested edit order (playlist)

| #   | Clip                          | Audience |
| --- | ----------------------------- | -------- |
| 1   | Unknown email → register      | Auth     |
| 2   | Register                      | Auth     |
| 3   | Request code + log + verify   | Auth     |
| 4   | Explore / My Recipes / detail | User     |
| 5   | Favorites                     | User     |
| 6   | Profile                       | User     |
| 7   | Logout                        | User     |
| 8   | Admin sidebar                 | Admin    |
| 9   | Admin users                   | Admin    |
| 10  | Admin recipes                 | Admin    |
| 11  | Admin lookups                 | Admin    |

**Total target:** ~5–8 minutes with voice-over, or silent clips with on-screen captions.
