/**
 * Utilitaire de gestion des erreurs API pour le site public CapLed.
 *
 * Le backend retourne désormais un format standardisé :
 * { "code": "LEAD_EMPTY_CART", "message": "..." }
 *
 * Cette fonction extrait le message le plus pertinent depuis :
 * 1. La réponse JSON du backend (code connu → message surchargé FR)
 * 2. Le message backend directement (déjà en français)
 * 3. Un fallback générique si tout échoue
 */

// Mapping code → message utilisateur personnalisé (FR)
const API_ERROR_MAP: Record<string, string> = {
  // Auth
  INVALID_CREDENTIALS: 'Adresse e-mail ou mot de passe incorrect.',
  EMAIL_ALREADY_EXISTS: 'Cette adresse e-mail est déjà associée à un compte.',
  EMAIL_NOT_CONFIRMED: 'Veuillez confirmer votre adresse e-mail avant de vous connecter. Vérifiez votre boîte de réception.',
  CLIENT_NOT_FOUND: 'Aucun compte trouvé pour cette adresse e-mail.',
  TOKEN_INVALID: 'Le lien de confirmation est invalide ou a déjà été utilisé.',
  TOKEN_EXPIRED: 'Le lien de confirmation a expiré. Demandez un nouveau lien de confirmation.',
  // Devis / Leads
  LEAD_EMPTY_CART: 'Votre demande de devis est vide. Ajoutez au moins un article.',
  LEAD_NOT_FOUND: 'La demande de devis est introuvable.',
  LEAD_ARTICLE_INVALID: 'Un ou plusieurs articles de votre demande sont invalides.',
  // Stock
  STOCK_INSUFFICIENT: 'Stock insuffisant pour traiter cette opération.',
  ARTICLE_NOT_FOUND: 'L\'article demandé est introuvable.',
  // Serveur
  INTERNAL_ERROR: 'Une erreur interne est survenue. Veuillez réessayer ou contacter le support.',
};

/**
 * Extrait un message d'erreur lisible depuis une erreur Axios ou une réponse API.
 * Ne jamais afficher de messages techniques à l'utilisateur.
 */
export function getErrorMessage(err: unknown): string {
  if (!err || typeof err !== 'object') {
    return 'Une erreur inattendue est survenue.';
  }

  const error = err as any;

  // Erreur réseau (pas de réponse du serveur)
  if (!error.response) {
    return 'Impossible de contacter le serveur. Vérifiez votre connexion et réessayez.';
  }

  const data = error.response?.data;

  // Format standardisé backend : { code, message }
  if (data?.code && typeof data.code === 'string') {
    // Priorité 1 : mapping local (message FR personnalisé)
    if (API_ERROR_MAP[data.code]) {
      return API_ERROR_MAP[data.code];
    }
    // Priorité 2 : message du backend (déjà en français)
    if (data.message && typeof data.message === 'string') {
      return data.message;
    }
  }

  // Anciens formats de réponse (compatibilité)
  if (data?.message && typeof data.message === 'string') return data.message;
  if (data?.error && typeof data.error === 'string') return data.error;

  // Fallback par code HTTP
  const status = error.response?.status;
  if (status === 401) return 'Vous n\'êtes pas connecté ou votre session a expiré.';
  if (status === 403) return 'Vous n\'avez pas les droits pour effectuer cette action.';
  if (status === 404) return 'La ressource demandée est introuvable.';
  if (status === 409) return 'Un conflit a été détecté. Vérifiez les données saisies.';
  if (status === 422) return 'Les données saisies sont invalides. Vérifiez le formulaire.';
  if (status && status >= 500) return 'Une erreur interne est survenue. Veuillez réessayer plus tard.';

  return 'Une erreur inattendue est survenue.';
}
